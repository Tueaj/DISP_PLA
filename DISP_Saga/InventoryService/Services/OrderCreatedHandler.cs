using EventLibrary;
using InventoryService.Models;
using MessageHandling;
using MessageHandling.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace InventoryService.Services;

public class OrderCreatedHandler : EventHandler<OrderCreated>
{
    private readonly ILogger<OrderCreatedHandler> _logger;
    private readonly IMessageProducer _producer;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IReservationRepository _reservationRepository;

    public OrderCreatedHandler(ILogger<OrderCreatedHandler> logger, IMessageProducer producer,
        IInventoryRepository inventoryRepository, IReservationRepository reservationRepository)
    {
        _logger = logger;
        _producer = producer;
        _inventoryRepository = inventoryRepository;
        _reservationRepository = reservationRepository;
    }

    public override void Handle(OrderCreated message)
    {
        _logger.LogInformation(message.ToJson());

        foreach (var entry in message.OrderedItems)
        {
            var foundItem = _inventoryRepository.GetItemByName(entry.Key);

            if (foundItem == default || foundItem.Amount < entry.Value)
            {
                _producer.ProduceMessage(new InventoryReservationFailed {OrderId = message.OrderId}, QueueName.Command);
                return;
            }
        }

        Reservation reservation = new(){ OrderId = message.OrderId};

        foreach (var entry in message.OrderedItems)
        {
            var foundItem = _inventoryRepository.GetItemByName(entry.Key);

            foundItem.Amount -= entry.Value;
            
            _inventoryRepository.UpdateItem(foundItem);
            reservation.ItemReservations.Add(foundItem);
        }

        _reservationRepository.CreateReservation(reservation);

        _producer.ProduceMessage(new InventoryReserved {OrderId = message.OrderId}, QueueName.Command);
    }
}