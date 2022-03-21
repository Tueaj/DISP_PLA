using EventLibrary;
using MessageHandling;
using MessageHandling.Abstractions;
using Microsoft.Extensions.Logging;
using System;

namespace CreditService.Services;

public class OrderCreatedHandler : EventLibrary.EventHandler<OrderCreated>
{
    private readonly ILogger<OrderCreatedHandler> _logger;
    private readonly IMessageProducer _producer;
    private readonly ICreditLogic _creditLogic;

    public OrderCreatedHandler(ILogger<OrderCreatedHandler> logger, IMessageProducer producer,
        ICreditLogic creditLogic)
    {
        _logger = logger;
        _producer = producer;
        _creditLogic = creditLogic;
    }

    public override void Handle(OrderCreated message)
    {
        try
        {
            _creditLogic.OrderCreated(message);
            _producer.ProduceMessage(new CreditReserved { OrderId = message.OrderId }, QueueName.Command);
        }
        catch (Exception exception)
        {
            //Should check whether exception is custom exception, so it can log something other than error etc.
            _logger.LogError("CreditService OrderCreatedHandler - failed with exception: " + exception);
            _producer.ProduceMessage(new CreditReservationFailed() { OrderId = message.OrderId }, QueueName.Command);
        }
    }
}