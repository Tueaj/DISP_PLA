﻿using EventLibrary;
using MessageHandling;
using MessageHandling.Abstractions;
using Microsoft.Extensions.Logging;
using System;

namespace InventoryService.Services;

public class OrderCreatedHandler : EventLibrary.EventHandler<OrderCreated>
{
    private readonly ILogger<OrderCreatedHandler> _logger;
    private readonly IMessageProducer _producer;
    private readonly IInventoryLogic _inventoryLogic;

    public OrderCreatedHandler(ILogger<OrderCreatedHandler> logger, IMessageProducer producer,
        IInventoryLogic inventoryLogic)
    {
        _logger = logger;
        _producer = producer;
        _inventoryLogic = inventoryLogic;
    }

    public override void Handle(OrderCreated message)
    {
        try
        {
            _inventoryLogic.OrderCreated(message);
        }
        catch (Exception exception)
        {
            _logger.LogError("InventoryService OrderCreatedHandler - failed with exception: " + exception);
            _producer.ProduceMessage(new InventoryReservationFailed() { OrderId = message.OrderId }, QueueName.Command);
        }
    }
}