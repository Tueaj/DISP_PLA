using System.Linq;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OrderService.Models;

namespace OrderService.Services.Handlers
{
    public class AbortInventoryAckHandler : CommandHandler<AbortInventoryAck>
    {
        private readonly ILogger<AbortInventoryAckHandler> _logger;
        private readonly IOrderRepository _orderRepository;

        public AbortInventoryAckHandler(
            ILogger<AbortInventoryAckHandler> logger,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        public override void Handle(AbortInventoryAck message)
        {
            _logger.LogInformation(message.ToJson());

            var order = _orderRepository.GetOrderById(message.TransactionId);

            var inventoryItem = order.Inventory.First(i => i.ItemId == message.ItemId);

            inventoryItem.Status = TransactionStatus.Aborted;

            _orderRepository.UpdateOrder(order);
        }
    }
}