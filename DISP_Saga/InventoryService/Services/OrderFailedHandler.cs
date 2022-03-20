using EventLibrary;
using InventoryService.Models;
using MessageHandling;
using MessageHandling.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using System;

namespace InventoryService.Services;

public class OrderFailedHandler : EventLibrary.EventHandler<OrderFailed>
{
    private readonly ILogger<OrderFailedHandler> _logger;
    private readonly IMessageProducer _producer;
    private readonly IInventoryLogic _inventoryLogic;

    public OrderFailedHandler(ILogger<OrderFailedHandler> logger, IMessageProducer producer,
        IInventoryLogic inventoryLogic)
    {
        _logger = logger;
        _producer = producer;
        _inventoryLogic = inventoryLogic;
    }

    public override void Handle(OrderFailed message)
    {
        try
        {
            _inventoryLogic.OrderFailed(message);
        }
        catch (Exception exception)
        {
            _logger.LogError("InventoryService OrderFailedHandler - failed with exception: " + exception);
        }
    }
}