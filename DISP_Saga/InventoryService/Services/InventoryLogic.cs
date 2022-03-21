using InventoryService.Models;
using EventLibrary;
using MessageHandling;
using MessageHandling.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace InventoryService.Services
{
    public class InventoryLogic : IInventoryLogic
    {
        private readonly ILogger<InventoryLogic> _logger;
        private readonly IMessageProducer _producer;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly MongoClient _mongoClient;


        public InventoryLogic(ILogger<InventoryLogic> logger, IMessageProducer producer,
            IInventoryRepository inventoryRepository, IReservationRepository reservationRepository,
            IOptions<MongoConnectionSettings> settings)
        {
            _logger = logger;
            _producer = producer;
            _inventoryRepository = inventoryRepository;
            _reservationRepository = reservationRepository;
            _mongoClient = new MongoClient($"mongodb://{settings.Value.HostName}:{settings.Value.Port}");
        }
        public void OrderCreated(OrderCreated message)
        {
            _logger.LogInformation(message.ToJson());

            foreach (var entry in message.OrderedItems)
            {
                var foundItem = _inventoryRepository.GetItemByName(entry.Key);

                if (foundItem == default || foundItem.Amount < entry.Value)
                {
                    _producer.ProduceMessage(new InventoryReservationFailed { OrderId = message.OrderId }, QueueName.Command);
                    return;
                }
            }

            Reservation reservation = new() { OrderId = message.OrderId };

            using (var transaction = _mongoClient.StartSession())
            {
                transaction.StartTransaction();

                try
                {
                    foreach (var entry in message.OrderedItems)
                    {
                        var foundItem = _inventoryRepository.GetItemByName(entry.Key);

                        foundItem.Amount -= entry.Value;

                        _inventoryRepository.UpdateItem(foundItem);
                        reservation.ItemReservations.Add(foundItem);
                    }

                    _reservationRepository.CreateReservation(reservation);

                    transaction.CommitTransaction();
                }
                catch (Exception)
                {
                    transaction.AbortTransaction();
                    throw;
                }
            }

            _producer.ProduceMessage(new InventoryReserved { OrderId = message.OrderId }, QueueName.Command);
        }

        public void OrderFailed(OrderFailed message)
        {
            _logger.LogInformation(message.ToJson());

            Reservation? reservation = _reservationRepository.GetReservation(message.OrderId);

            if (reservation == null)
            {
                return;
            }

            using (var transaction = _mongoClient.StartSession())
            {
                transaction.StartTransaction();

                try
                {
                    foreach (var reservedItem in reservation.ItemReservations)
                    {
                        Item? item = _inventoryRepository.GetItemByName(reservedItem.Name);

                        if (item == default)
                        {
                            continue;
                        }

                        item.Amount += reservedItem.Amount;

                        _inventoryRepository.UpdateItem(item);
                    }

                    _reservationRepository.DeleteReservation(reservation.OrderId);
                    transaction.CommitTransaction();
                }
                catch (Exception)
                {
                    transaction.AbortTransaction();
                    throw;
                }
            }
    }
}
