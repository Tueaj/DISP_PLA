using System;
using System.Collections.Generic;
using System.Data;
using InventoryService.Models;
using InventoryService.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InventoryService.Repository
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ILogger<InventoryRepository> _logger;
        private readonly IMongoCollection<Item> _inventoryCollection;

        public InventoryRepository(ILogger<InventoryRepository> logger, IOptions<MongoConnectionSettings> settings)
        {
            _logger = logger;
            IMongoClient mongoClient;

            if (settings.Value.Credentials == null)
            {
                mongoClient = new MongoClient($"mongodb://{settings.Value.HostName}:{settings.Value.Port}");
            }
            else
            {
                _logger.LogInformation("Using authenticated MongoDB connection");
                mongoClient =
                    new MongoClient(
                        $"mongodb://{settings.Value.Credentials.Username}:{settings.Value.Credentials.Password}@{settings.Value.HostName}:{settings.Value.Port}");
            }

            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

            _inventoryCollection = mongoDatabase.GetCollection<Item>("Item");
            
            var dummyDataService = new DummyDataService(this);
            dummyDataService.CreateDummyData();
        }

        public IEnumerable<Item> GetAllItems()
        {
            return _inventoryCollection.Find(_ => true).ToList();
        }

        public Item? GetItemById(string itemId)
        {
            return _inventoryCollection.Find(_ => _.ItemId == itemId).FirstOrDefault();
        }

        public bool ItemExists(string itemId)
        {
            return _inventoryCollection.CountDocuments(_ => _.ItemId == itemId) != 0;
        }

        public void CreateItem(Item item)
        {
            _inventoryCollection.InsertOne(item);
        }

        public void UpdateItem(Item item, string transactionId)
        {
            Item? updated = null;
            try
            {
                updated = _inventoryCollection.FindOneAndReplace(
                    _ => _.ItemId == item.ItemId && (_.Lock == null || _.Lock.LockedBy == transactionId), item);
            }
            catch (MongoException e) when (e is MongoWriteConcernException or MongoWriteException)
            {
            }

            if (updated == null)
            {
                throw new ConstraintException($"Did not hold required lock for Inventory with id {item.ItemId}");
            }
        }

        public Item AcquireItem(string itemId, string transactionId)
        {
            Item? item = null;
            try
            {
                item = _inventoryCollection.FindOneAndUpdate(
                    _ => _.ItemId == itemId && (_.Lock == null || _.Lock.LockedBy == transactionId),
                    Builders<Item>.Update.Set(_ => _.Lock,
                        new ItemLock {LockedAt = DateTime.Now, LockedBy = transactionId}));
            }
            catch (MongoException e) when (e is MongoWriteConcernException or MongoWriteException)
            {
                _logger.LogError(e.Message);
            }

            if (item == null)
            {
                throw new ConstraintException($"Could not acquire lock for Inventory with ID {itemId}");
            }

            return item;
        }

        public void ReleaseItem(string itemId, string transactionId)
        {
            Item? item = null;
            try
            {
                item = _inventoryCollection.FindOneAndUpdate(
                    _ => _.ItemId == itemId && (_.Lock == null || _.Lock.LockedBy == transactionId),
                    Builders<Item>.Update.Set(_ => _.Lock, null));
            }
            catch (MongoException e) when (e is MongoWriteConcernException or MongoWriteException)
            {
            }

            if (item == null)
            {
                throw new ConstraintException($"Could not release lock for Inventory with ID {itemId}");
            }
        }

        public List<Item> AcquireOldLocks(string transactionId)
        {
            List<Item> expiredItems = _inventoryCollection
                .FindSync(_ => _.Lock != null && _.Lock.LockedAt.CompareTo(DateTime.Now.AddMinutes(-1)) >= 0).ToList();

            List<Item> lockedItems = new List<Item>();
            foreach (var expiredItem in expiredItems)
            {
                Item? tryLock = null;
                try
                {
                    tryLock = _inventoryCollection.FindOneAndUpdate(
                        _ => _.ItemId == expiredItem.ItemId && _.Lock != null &&
                             _.Lock.LockedAt.CompareTo(DateTime.Now.AddMinutes(-1)) >= 0,
                        Builders<Item>.Update.Set(_ => _.Lock, new ItemLock
                        {
                            LockedAt = DateTime.Now,
                            LockedBy = transactionId
                        }));
                }
                catch (MongoException e) when (e is MongoWriteConcernException or MongoWriteException)
                {
                }

                if (tryLock != null)
                {
                    lockedItems.Add(tryLock);
                }
            }

            return lockedItems;
        }
    }
}