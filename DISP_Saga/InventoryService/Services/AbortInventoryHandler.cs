using System.Linq;
using InventoryService.Models;
using InventoryService.Repository;
using MessageHandling;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace InventoryService.Services
{
    public class AbortInventoryHandler : CommandHandler<AbortInventory>
    {
        private readonly ILogger<AbortInventoryHandler> _logger;
        private readonly IMessageProducer _producer;
        private readonly IInventoryRepository _inventoryRepository;

        public AbortInventoryHandler(
            ILogger<AbortInventoryHandler> logger,
            IMessageProducer producer,
            IInventoryRepository inventoryRepository)
        {
            _logger = logger;
            _producer = producer;
            _inventoryRepository = inventoryRepository;
        }

        public override void Handle(AbortInventory message)
        {
            _logger.LogInformation(message.ToJson());

            if (_inventoryRepository.ItemExists(message.ItemId))
            {
                // Ignore the constraint exception - If we fail to acquire the lock, we should just throw, re-queue,
                // and try again.
                Item item = _inventoryRepository.AcquireItem(message.ItemId, message.TransactionId);

                var change = item.ChangeLog.FirstOrDefault(log => log.TransactionId == message.TransactionId);
                if (change != default)
                {
                    change.Status = ItemChangeStatus.Aborted;
                    _inventoryRepository.UpdateItem(item, message.TransactionId);
                }
                
                _inventoryRepository.ReleaseItem(item.ItemId, message.TransactionId);
            }
            _producer.ProduceMessage(
                new AbortInventoryAck {TransactionId = message.TransactionId, ItemId = message.ItemId},
                QueueName.Command);
        }
    }
}