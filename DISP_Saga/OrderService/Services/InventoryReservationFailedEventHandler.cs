using OrderService.Models;

namespace OrderService.Services;

public class InventoryReservationFailedEventHandler: EventHandler<InventoryReservationFailed>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMessageProducer _messageProducer;

    public InventoryReservationFailedEventHandler(
        IOrderRepository orderRepository, 
        IMessageProducer messageProducer)
    {
        _orderRepository = orderRepository;
        _messageProducer = messageProducer;
    }

    public override void Handle(InventoryReservationFailed message)
    {
        CreateOrderRequest? order = _orderRepository.GetOrderByOrderId(message.OrderId);

        if (order == default)
        {
            return;
        }
        
        var orderFailedEvent = new OrderFailed {OrderId = message.OrderId};
        _messageProducer.ProduceMessage(orderFailedEvent, "COMMAND");
        _orderRepository.DeleteOrder(message.OrderId);
    }
}