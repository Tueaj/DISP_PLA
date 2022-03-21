using CreditService.Models;
using EventLibrary;
using MessageHandling;
using MessageHandling.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;

namespace CreditService.Services;

public class OrderFailedHandler : EventLibrary.EventHandler<OrderFailed>
{
    private readonly ILogger<OrderFailedHandler> _logger;
    private readonly IMessageProducer _producer;
    private readonly ICreditLogic _creditLogic;

    public OrderFailedHandler(ILogger<OrderFailedHandler> logger, IMessageProducer producer,
        ICreditLogic creditLogic)
    {
        _logger = logger;
        _producer = producer;
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
        }
    }
}