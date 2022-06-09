using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using OrderService.Models;

namespace OrderService.Services.Handlers
{
    public class RollbackCreditAckHandler : CommandHandler<RollbackCreditAck>
    {
        private readonly ILogger<RollbackCreditAckHandler> _logger;
        private readonly IOrderRepository _orderRepository;

        public RollbackCreditAckHandler(
            ILogger<RollbackCreditAckHandler> logger,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        public override void Handle(RollbackCreditAck message)
        {
            _logger.LogInformation(message.ToJson());
            
            var order = _orderRepository.GetOrderById(message.TransactionId);
            
            order.Credit.Status = TransactionStatus.Rolledback;
            
            _orderRepository.UpdateOrder(order);
        }
    }
}