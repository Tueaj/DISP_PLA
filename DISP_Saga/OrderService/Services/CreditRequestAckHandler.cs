using MessageHandling.Abstractions;
using Messages;
using OrderService.Models;

namespace OrderService.Services
{
    public class CreditRequestAckHandler: CommandHandler<CreditRequestAck>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderStatusService _orderStatusService;

        public CreditRequestAckHandler(
            IOrderRepository orderRepository, 
            OrderStatusService orderStatusService)
        {
            _orderRepository = orderRepository;
            _orderStatusService = orderStatusService;
        }

        public override void Handle(CreditRequestAck message)
        {
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