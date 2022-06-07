using MessageHandling.Abstractions;
using Messages;
using OrderService.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace OrderService.Services
{
    public class CommitCreditAckHandler : CommandHandler<CommitCreditAck>
    {
        private readonly ILogger<CommitCreditAckHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly OrderStatusService _orderStatusService;

        public CommitCreditAckHandler(
            ILogger<CommitCreditAckHandler> logger,
            IOrderRepository orderRepository,
            OrderStatusService orderStatusService)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _orderStatusService = orderStatusService;
        }

        public override void Handle(CommitCreditAck message)
        {
            _logger.LogInformation(message.ToJson());

            var order = _orderRepository.GetOrderById(message.OrderId);

            if (order.Credit.Status == TransactionStatus.REQUESTED)
            {
                order.Credit.Status = TransactionStatus.COMMITTED;
            }

            _orderRepository.UpdateOrder(order);

            _orderStatusService.OrderUpdated(order.OrderId);
        }
    }
}