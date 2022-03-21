using EventLibrary;
using MessageHandling.Abstractions;
using Microsoft.Extensions.Logging;

namespace OrderService.Services;

public class CreditReservedEventHandler: EventHandler<CreditReserved>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMessageProducer _messageProducer;

    public CreditReservedEventHandler(
        IOrderRepository orderRepository, 
        IMessageProducer messageProducer)
    {
        _orderRepository = orderRepository;
        _messageProducer = messageProducer;
    }

    public override void Handle(CreditReserved message)
    {
        if(_orderRepository.CheckOrderFailed(message.OrderId))
        {
            var orderFailedEvent = new OrderFailed { OrderId = message.OrderId };
            _messageProducer.ProduceMessage(orderFailedEvent, "COMMAND");
        }

        _orderRepository.CreditReserved(message.OrderId);
        if (!_orderRepository.CheckOrderComplete(message.OrderId))
        {
            return;
        }
        
        var order = _orderRepository.GetOrderByOrderId(message.OrderId);
        var orderSucceededEvent = new OrderSucceeded
        {
            OrderId = order.OrderId,
            OrderedItems = order.OrderedItems
        };
        _messageProducer.ProduceMessage(orderSucceededEvent, "COMMAND");
    }
}