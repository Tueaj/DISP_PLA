using System;
using InventoryService.Models;
using MessageHandling;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace InventoryService.Services
{
    public class InventoryRequestHandler : CommandHandler<InventoryRequest>
    {
        private readonly ILogger<InventoryRequestHandler> _logger;
        private readonly IMessageProducer _producer;
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryRequestHandler(ILogger<InventoryRequestHandler> logger, IMessageProducer producer,
            IInventoryRepository inventoryRepository)
        {
            _logger = logger;
            _producer = producer;
            _inventoryRepository = inventoryRepository;
        }
        
        public override void Handle(InventoryRequest message)
        {
            _logger.LogInformation(message.ToJson());

            Item? item = _inventoryRepository.GetItemById(message.ItemId);
            
            if (item.PendingReservation is not null)
            {
                throw new Exception();
            }

            if (item == null || item.Amount < message.Amount)
            {
                //produce message for abort
                return;
            }
            
            
            _inventoryRepository.SetReservationOnItem(message);
            
            _producer.ProduceMessage(new InventoryRequestAck {TransactionId = message.TransactionId, ItemId = message.ItemId}, QueueName.Command);
        }
    }
}