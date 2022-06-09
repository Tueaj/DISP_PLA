using System.Linq;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OrderService.Models;

namespace OrderService.Services.Handlers
{
    public class InventoryRequestNackHandler : CommandHandler<InventoryRequestNack>
    {
        private readonly ILogger<InventoryRequestNackHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly OrderStatusService _orderStatusService;

        public InventoryRequestNackHandler(
            ILogger<InventoryRequestNackHandler> logger,
            IOrderRepository orderRepository,
            OrderStatusService orderStatusService)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _orderStatusService = orderStatusService;
        }

        public override void Handle(InventoryRequestNack message)
        {
            _logger.LogInformation(message.ToJson());

            var order = _orderRepository.GetOrderById(message.TransactionId);

            var inventoryItem = order.Inventory.First(i => i.ItemId == message.ItemId);

            inventoryItem.Status = TransactionStatus.Aborted;

            _orderRepository.UpdateOrder(order);

            _orderStatusService.OrderUpdated(order.TransactionId);
        }
    }
}