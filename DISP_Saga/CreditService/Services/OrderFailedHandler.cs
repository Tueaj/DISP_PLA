using EventLibrary;
using MessageHandling.Abstractions;
using Microsoft.Extensions.Logging;
using System;

namespace CreditService.Services;

public class OrderFailedHandler : EventLibrary.EventHandler<OrderFailed>
{
    private readonly ILogger<OrderFailedHandler> _logger;
    private readonly ICreditLogic _creditLogic;

    public OrderFailedHandler(ILogger<OrderFailedHandler> logger, ICreditLogic creditLogic)
    {
        _logger = logger;
        _creditLogic = creditLogic;
    }

    public override void Handle(OrderFailed message)
    {
        try
        {
            _creditLogic.OrderFailed(message);
        }
        catch (Exception exception)
        {
            _logger.LogError("CreditService OrderFailedHandler - failed with exception: " + exception);
            //Throw back to RabbitMQ for retry
            throw;
        }
    }
}