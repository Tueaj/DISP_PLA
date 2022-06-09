using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OrderService.Models;

namespace OrderService.Services.Handlers
{
    public class CommitCreditNackHandler : CommandHandler<CommitCreditNack>
    {
        private readonly ILogger<CommitCreditNackHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly OrderStatusService _orderStatusService;

        public CommitCreditNackHandler(
            ILogger<CommitCreditNackHandler> logger,
            IOrderRepository orderRepository,
            OrderStatusService orderStatusService)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _orderStatusService = orderStatusService;
        }

        public override void Handle(CommitCreditNack message)
        {
            _logger.LogInformation(message.ToJson());

            var order = _orderRepository.GetOrderById(message.TransactionId);

            order.Credit.Status = TransactionStatus.Rollback;
            order.Status = OrderStatus.FAILED;

            _orderRepository.UpdateOrder(order);

            _orderStatusService.OrderUpdated(order.TransactionId);
        }
    }
}