using System.Linq;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OrderService.Models;

namespace OrderService.Services.Handlers
{
    public class CommitInventoryNackHandler : CommandHandler<CommitInventoryNack>
    {
        private readonly ILogger<CommitInventoryNackHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly OrderStatusService _orderStatusService;

        public CommitInventoryNackHandler(
            ILogger<CommitInventoryNackHandler> logger,
            IOrderRepository orderRepository,
            OrderStatusService orderStatusService)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _orderStatusService = orderStatusService;
        }

        public override void Handle(CommitInventoryNack message)
        {
            _logger.LogInformation(message.ToJson());

            var order = _orderRepository.GetOrderById(message.TransactionId);

            var nackedItem = order.Inventory.FirstOrDefault(item => item.ItemId == message.ItemId);

            if (nackedItem is null)
            {
                _logger.LogError("Inventory nack on item that does not exits on order");
                return;
            }

            nackedItem.Status = TransactionStatus.Rollback;
            order.Status = OrderStatus.FAILED;

            _orderRepository.UpdateOrder(order);

            _orderStatusService.OrderUpdated(order.TransactionId);
        }
    }
}