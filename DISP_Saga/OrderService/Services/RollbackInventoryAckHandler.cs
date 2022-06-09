using System.Linq;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OrderService.Models;

namespace OrderService.Services
{
    public class RollbackInventoryAckHandler : CommandHandler<RollbackInventoryAck>
    {
        private readonly ILogger<RollbackInventoryAckHandler> _logger;
        private readonly IOrderRepository _orderRepository;

        public RollbackInventoryAckHandler(
            ILogger<RollbackInventoryAckHandler> logger,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        public override void Handle(RollbackInventoryAck message)
        {
            _logger.LogInformation(message.ToJson());
            
            var order = _orderRepository.GetOrderById(message.TransactionId);

            var inventoryItem = order.Inventory.First(i => i.ItemId == message.ItemId);

            inventoryItem.Status = TransactionStatus.Rolledback;
            
            _orderRepository.UpdateOrder(order);
        }
    }
}