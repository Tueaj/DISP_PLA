using System.Linq;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OrderService.Models;

namespace OrderService.Services.Handlers
{
    public class InventoryRequestAckHandler : CommandHandler<InventoryRequestAck>
    {
        private readonly ILogger<InventoryRequestAckHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly OrderStatusService _orderStatusService;

        public InventoryRequestAckHandler(
            ILogger<InventoryRequestAckHandler> logger,
            IOrderRepository orderRepository,
            OrderStatusService orderStatusService)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _orderStatusService = orderStatusService;
        }

        public override void Handle(InventoryRequestAck message)
        {
            _logger.LogInformation(message.ToJson());

            var order = _orderRepository.GetOrderById(message.TransactionId);

            var inventoryItem = order.Inventory.First(i => i.ItemId == message.ItemId);

            if (inventoryItem.Status == TransactionStatus.Pending)
            {
                inventoryItem.Status = TransactionStatus.Requested;
            }

            _orderRepository.UpdateOrder(order);

            _orderStatusService.OrderUpdated(order.TransactionId);
        }
    }
}