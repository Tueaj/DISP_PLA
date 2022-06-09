using InventoryService.Models;
using InventoryService.Repository;
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

            // Ignore the constraint exception - If we fail to acquire the lock, we should just throw, re-queue,
            // and try again.
            Item item = _inventoryRepository.AcquireItem(message.ItemId, message.TransactionId);

            var change = item.ChangeLog.Find(change => change.TransactionId == message.TransactionId);

            //Not sunshine
            if (change is not {Status: ItemChangeStatus.Pending})
            {
                //If transaction has been aborted, send NACK
                if (change == null || change.Status == ItemChangeStatus.Aborted)
                {
                    _producer.ProduceMessage(new CommitInventoryNack
                        {
                            TransactionId = message.TransactionId,
                            ItemId = message.ItemId
                        },
                        QueueName.Command);
                }
                else
                {
                    //If transaction has been rolled back, it has also been committed. Send ACK
                    _producer.ProduceMessage(
                        new CommitInventoryAck {TransactionId = message.TransactionId, ItemId = message.ItemId},
                        QueueName.Command);
                }

                _inventoryRepository.ReleaseItem(message.ItemId, message.TransactionId);

                return;
            }
            
            //Sunshine
            item.Amount -= change.Amount;
            change.Status = ItemChangeStatus.Performed;

            _inventoryRepository.UpdateItem(item, message.TransactionId);
            _inventoryRepository.ReleaseItem(message.ItemId, message.TransactionId);

            _producer.ProduceMessage(
                new CommitInventoryAck {TransactionId = message.TransactionId, ItemId = message.ItemId},
                QueueName.Command);
        }
    }
}