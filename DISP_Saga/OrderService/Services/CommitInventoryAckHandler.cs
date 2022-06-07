using System.Linq;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OrderService.Models;

namespace OrderService.Services
{
    public class CommitInventoryAckHandler : CommandHandler<CommitInventoryAck>
    {
        private readonly ILogger<CommitInventoryAckHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly OrderStatusService _orderStatusService;

        public CommitInventoryAckHandler(
            ILogger<CommitInventoryAckHandler> logger,
            IOrderRepository orderRepository,
            OrderStatusService orderStatusService)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _orderStatusService = orderStatusService;
        }

        public override void Handle(CommitInventoryAck message)
        {
            _logger.LogInformation(message.ToJson());

            var order = _orderRepository.GetOrderById(message.OrderId);

            var inventoryItem = order.Inventory.First(i => i.ItemId == message.ItemId);

            if (inventoryItem.Status == TransactionStatus.REQUESTED)
            {
                inventoryItem.Status = TransactionStatus.COMMITTED;
            }

            _orderRepository.UpdateOrder(order);

            _orderStatusService.OrderUpdated(order.OrderId);
        }
    }
}