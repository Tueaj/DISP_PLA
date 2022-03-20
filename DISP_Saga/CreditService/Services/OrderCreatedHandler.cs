using CreditService.Models;
using EventLibrary;
using MessageHandling;
using MessageHandling.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
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
        }
        catch (Exception exception)
        {
            _logger.LogError("CreditService OrderCreatedHandler - failed with exception: " + exception);
            _producer.ProduceMessage(new CreditReservationFailed() { OrderId = message.OrderId }, QueueName.Command);
        }
    }
}