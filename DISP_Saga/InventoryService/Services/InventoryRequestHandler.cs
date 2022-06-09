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

            if (!_inventoryRepository.ItemExists(message.ItemId))
            {
                _producer.ProduceMessage(new InventoryRequestNack()
                {
                    ItemId = message.ItemId,
                    TransactionId = message.TransactionId
                }, QueueName.Command);
                return;
            }
            
            // Ignore the constraint exception - If we fail to acquire the lock, we should just throw, re-queue,
            // and try again.
            Item item = _inventoryRepository.AcquireItem(message.ItemId, message.TransactionId);
            
            var existingChange = item.ChangeLog.FirstOrDefault(log => log.TransactionId == message.TransactionId);
            
            if (existingChange != default)
            {
                //Transaction has taken item, but never completes and is therefore aborted by TimeoutDetector
                if (existingChange.Status == ItemChangeStatus.Aborted)
                {
                    _producer.ProduceMessage(new InventoryRequestNack()
                    {
                        ItemId = message.ItemId,
                        TransactionId = message.TransactionId
                    }, QueueName.Command);
                }
                else
                {
                    //Transaction has been rolled back, which means it has been committed, so send ACK
                    _producer.ProduceMessage(
                        new InventoryRequestAck {TransactionId = message.TransactionId, ItemId = message.ItemId},
                        QueueName.Command);
                }

                //If transaction is pending, don't do anything, else release lock on item.
                if (existingChange.Status != ItemChangeStatus.Pending)
                {
                    _inventoryRepository.ReleaseItem(message.ItemId, message.TransactionId);
                }

                return;
            }
            
            var itemChange = new ItemChange
            {
                Amount = message.Amount,
                Status = ItemChangeStatus.Pending,
                TransactionId = message.TransactionId
            };

            item.ChangeLog.Add(itemChange);

            _inventoryRepository.UpdateItem(item, message.TransactionId);
            
            if (item.Amount < message.Amount)
            {
                _inventoryRepository.ReleaseItem(message.ItemId, message.TransactionId);
                _producer.ProduceMessage(new InventoryRequestNack()
                {
                    ItemId = message.ItemId,
                    TransactionId = message.TransactionId
                }, QueueName.Command);
                return;
            }

            // Don't release lock on item, we need to keep it until the commit is completed

            _producer.ProduceMessage(
                new InventoryRequestAck {TransactionId = message.TransactionId, ItemId = message.ItemId},
                QueueName.Command);
        }
    }
}