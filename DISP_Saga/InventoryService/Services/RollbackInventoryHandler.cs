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
    public class RollbackInventoryHandler : CommandHandler<RollbackInventory>
    {
        private readonly ILogger<RollbackInventoryHandler> _logger;
        private readonly IMessageProducer _producer;
        private readonly IInventoryRepository _inventoryRepository;

        public RollbackInventoryHandler(
            ILogger<RollbackInventoryHandler> logger,
            IMessageProducer producer,
            IInventoryRepository inventoryRepository)
        {
            _logger = logger;
            _producer = producer;
            _inventoryRepository = inventoryRepository;
        }

        public override void Handle(RollbackInventory message)
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
                    if (change.Status == ItemChangeStatus.Performed)
                    {
                        item.Amount += change.Amount;
                        change.Status = ItemChangeStatus.RolledBack;
                    }

                    if (change.Status == ItemChangeStatus.Pending)
                    {
                        change.Status = ItemChangeStatus.RolledBack;
                    }

                    _inventoryRepository.UpdateItem(item, message.TransactionId);
                }

                _inventoryRepository.ReleaseItem(item.ItemId, message.TransactionId);
            }
            _producer.ProduceMessage(
                new RollbackInventoryAck {TransactionId = message.TransactionId, ItemId = message.ItemId},
                QueueName.Command);
        }
    }
}