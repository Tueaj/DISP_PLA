using System;
using System.Linq;
using MessageHandling;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using OrderService.Models;

namespace OrderService.Services
{
    public class OrderStatusService
    {
        private event EventHandler<OrderStatusUpdatedEventArgs> OrderStatusUpdated;

        private readonly ILogger<OrderStatusService> _logger;
        private readonly IOrderRepository _repository;
        private readonly IMessageProducer _messageProducer;

        public OrderStatusService(
            ILogger<OrderStatusService> logger,
            IOrderRepository repository,
            IMessageProducer messageProducer
        )
        {
            _logger = logger;
            _repository = repository;
            _messageProducer = messageProducer;
            OrderStatusUpdated += OnOrderStatusUpdated;
        }

        void OnOrderStatusUpdated(object? sender, OrderStatusUpdatedEventArgs orderStatusUpdatedEventArgs)
        {
            _logger.LogInformation("OrderStatusUpdated triggered");
            var order = _repository.GetOrderById(orderStatusUpdatedEventArgs.OrderId);

            if (order.Credit.Status == TransactionStatus.Requested &&
                order.Inventory.All(item => item.Status == TransactionStatus.Requested))
            {
                _logger.LogInformation("Starting commit - Credit with id {}", order.Credit.CreditId);

                _messageProducer.ProduceMessage(new CommitCredit
                {
                    TransactionId = order.TransactionId,
                    CreditId = order.Credit.CreditId
                }, QueueName.Command);
                foreach (var inventoryState in order.Inventory)
                {
                    _logger.LogInformation("Starting commit - Item with id {}", inventoryState.ItemId);

                    _messageProducer.ProduceMessage(new CommitInventory
                    {
                        TransactionId = order.TransactionId,
                        ItemId = inventoryState.ItemId
                    }, QueueName.Command);
                }
            }
            else if (order.Credit.Status == TransactionStatus.Committed &&
                     order.Inventory.All(item => item.Status == TransactionStatus.Committed))
            {
                _logger.LogInformation("Ship order {}...", order.TransactionId);

                _messageProducer.ProduceMessage(new ShipOrder
                {
                    TransactionId = order.TransactionId,
                    ItemsToShip = order.Inventory.ToDictionary(item => item.ItemId, item => item.Amount)
                }, QueueName.Command);
            }
            else if (order.Credit.Status == TransactionStatus.Abort ||
                     order.Inventory.Any(item => item.Status == TransactionStatus.Abort))
            {
                _logger.LogInformation("Aborting order - transaction id {}", order.TransactionId);

                _messageProducer.ProduceMessage(new AbortCredit()
                {
                    TransactionId = order.TransactionId,
                    CreditId = order.Credit.CreditId
                }, QueueName.Command);

                foreach (var item in order.Inventory)
                {
                    _messageProducer.ProduceMessage(new AbortInventory()
                    {
                        TransactionId = order.TransactionId,
                        ItemId = item.ItemId
                    }, QueueName.Command);
                }
            }
            else if (order.Credit.Status == TransactionStatus.Rollback ||
                     order.Inventory.Any(item => item.Status == TransactionStatus.Rollback))
            {
                _logger.LogInformation("Aborting order - transaction id {}", order.TransactionId);

                _messageProducer.ProduceMessage(new RollbackCredit()
                {
                    TransactionId = order.TransactionId,
                    CreditId = order.Credit.CreditId
                }, QueueName.Command);

                foreach (var item in order.Inventory)
                {
                    _messageProducer.ProduceMessage(new RollbackInventory()
                    {
                        TransactionId = order.TransactionId,
                        ItemId = item.ItemId
                    }, QueueName.Command);
                }
            }
        }


        public void OrderUpdated(string orderId)
        {
            OrderStatusUpdated(this, new OrderStatusUpdatedEventArgs
            {
                OrderId = orderId
            });
        }
    }

    public class OrderStatusUpdatedEventArgs : EventArgs
    {
        public string OrderId;
    }
}