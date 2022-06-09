using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OrderService.Models;

namespace OrderService.Services.Handlers
{
    public class AbortCreditAckHandler : CommandHandler<AbortCreditAck>
    {
        private readonly ILogger<AbortCreditAckHandler> _logger;
        private readonly IOrderRepository _orderRepository;

        public AbortCreditAckHandler(
            ILogger<AbortCreditAckHandler> logger,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        public override void Handle(AbortCreditAck message)
        {
            _logger.LogInformation(message.ToJson());

            var order = _orderRepository.GetOrderById(message.TransactionId);

            order.Credit.Status = TransactionStatus.Aborted;

            _orderRepository.UpdateOrder(order);
        }
    }
}