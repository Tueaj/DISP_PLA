using MessageHandling.Abstractions;
using Messages;
using OrderService.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace OrderService.Services
{
    public class CreditRequestNackHandler : CommandHandler<CreditRequestNack>
    {
        private readonly ILogger<CreditRequestNackHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly OrderStatusService _orderStatusService;

        public CreditRequestNackHandler(
            ILogger<CreditRequestNackHandler> logger,
            IOrderRepository orderRepository,
            OrderStatusService orderStatusService)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _orderStatusService = orderStatusService;
        }

        public override void Handle(CreditRequestNack message)
        {
            _logger.LogInformation(message.ToJson());

            var order = _orderRepository.GetOrderById(message.TransactionId);
            
            order.Credit.Status = TransactionStatus.Aborted;
            
            _orderRepository.UpdateOrder(order);

            _orderStatusService.OrderUpdated(order.TransactionId);
        }
    }
}