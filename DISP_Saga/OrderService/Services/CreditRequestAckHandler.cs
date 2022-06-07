using MessageHandling.Abstractions;
using Messages;
using OrderService.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace OrderService.Services
{
    public class CreditRequestAckHandler : CommandHandler<CreditRequestAck>
    {
        private readonly ILogger<CreditRequestAckHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly OrderStatusService _orderStatusService;

        public CreditRequestAckHandler(
            ILogger<CreditRequestAckHandler> logger,
            IOrderRepository orderRepository,
            OrderStatusService orderStatusService)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _orderStatusService = orderStatusService;
        }

        public override void Handle(CreditRequestAck message)
        {
            _logger.LogInformation(message.ToJson());

            var order = _orderRepository.GetOrderById(message.OrderId);

            if (order.Credit.Status == TransactionStatus.PENDING)
            {
                order.Credit.Status = TransactionStatus.REQUESTED;
            }

            _orderRepository.UpdateOrder(order);

            _orderStatusService.OrderUpdated(order.OrderId);
        }
    }
}