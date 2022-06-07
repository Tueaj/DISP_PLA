using MessageHandling.Abstractions;
using Messages;
using OrderService.Models;

namespace OrderService.Services
{
    public class ShipOrderAckHandler : CommandHandler<ShipOrderAck>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly OrderStatusService _orderStatusService;

        public ShipOrderAckHandler(
            IOrderRepository orderRepository,
            OrderStatusService orderStatusService)
        {
            _orderRepository = orderRepository;
            _orderStatusService = orderStatusService;
        }

        public override void Handle(ShipOrderAck message)
        {
            var order = _orderRepository.GetOrderById(message.OrderId);

            order.Status = OrderStatus.COMPLETED;

            _orderRepository.UpdateOrder(order);
        }
    }
}