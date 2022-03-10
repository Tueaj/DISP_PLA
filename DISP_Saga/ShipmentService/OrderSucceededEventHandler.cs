using EventLibrary;
using Microsoft.Extensions.Logging;

namespace ShipmentService;

class OrderSucceededEventHandler : EventHandler<OrderSucceeded>
{
    private readonly ILogger _logger;

    public OrderSucceededEventHandler(ILogger logger)
    {
        _logger = logger;
    }

    public override void Handle(OrderSucceeded message)
    {
        _logger.LogInformation($"Order succeeded with id: {message.OrderId} and items : {message.OrderedItems}");
    }
}