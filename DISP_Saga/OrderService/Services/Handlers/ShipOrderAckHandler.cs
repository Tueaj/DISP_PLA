using MessageHandling.Abstractions;
using Messages;
using OrderService.Models;

namespace OrderService.Services.Handlers
{
    public class ShipOrderAckHandler : CommandHandler<ShipOrderAck>
    {
        private readonly IOrderRepository _orderRepository;

        public ShipOrderAckHandler(
            IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public override void Handle(ShipOrderAck message)
        {
            var order = _orderRepository.GetOrderById(message.TransactionId);

            order.Status = OrderStatus.COMPLETED;

            _orderRepository.UpdateOrder(order);
        }
    }
}