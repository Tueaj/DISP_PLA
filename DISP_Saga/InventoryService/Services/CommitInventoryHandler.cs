using InventoryService.Models;
using MessageHandling;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace InventoryService.Services
{
    public class CommitInventoryHandler : CommandHandler<CommitInventory>
    {
        private readonly ILogger<CommitInventoryHandler> _logger;
        private readonly IMessageProducer _producer;
        private readonly IInventoryRepository _inventoryRepository;

        public CommitInventoryHandler(ILogger<CommitInventoryHandler> logger, IMessageProducer producer,
            IInventoryRepository inventoryRepository)
        {
            _logger = logger;
            _producer = producer;
            _inventoryRepository = inventoryRepository;
        }

        public override void Handle(CommitInventory message)
        {
            _logger.LogInformation(message.ToJson());

            Item item = _inventoryRepository.GetItemById(message.ItemId);
        
            item.Amount -= item.PendingReservation.Amount;
            item.PendingReservation = null;
        
            _inventoryRepository.ReplaceItem(item);

            _producer.ProduceMessage(new CommitInventoryAck() {OrderId = message.OrderId, ItemId = message.ItemId}, QueueName.Command);
        }
    }
}