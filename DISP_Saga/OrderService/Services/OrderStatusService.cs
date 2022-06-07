using System;
using System.Collections.Generic;
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

            if (order.Credit.Status == TransactionStatus.REQUESTED &&
                order.Inventory.All(item => item.Status == TransactionStatus.REQUESTED))
            {
                _logger.LogInformation("Starting commit - Credit with id {}", order.Credit.CreditId);

                _messageProducer.ProduceMessage(new CommitCredit
                {
                    OrderId = order.OrderId,
                    CreditId = order.Credit.CreditId
                }, QueueName.Command);
                foreach (var inventoryState in order.Inventory)
                {                
                    _logger.LogInformation("Starting commit - Item with id {}", inventoryState.ItemId);

                    _messageProducer.ProduceMessage(new CommitInventory
                    {
                        OrderId = order.OrderId,
                        ItemId = inventoryState.ItemId
                    }, QueueName.Command);
                }
            }
            else if (order.Credit.Status == TransactionStatus.COMMITTED &&
                     order.Inventory.All(item => item.Status == TransactionStatus.COMMITTED))
            {
                _logger.LogInformation("Ship order...");

                _messageProducer.ProduceMessage(new ShipOrder
                {
                    OrderId = order.OrderId,
                    ItemsToShip = order.Inventory.ToDictionary(item => item.ItemId, item => item.Amount)
                }, QueueName.Command);
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