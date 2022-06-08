using System.Collections.Generic;
using InventoryService.Models;
using Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InventoryService.Services;

public class InventoryRepository : IInventoryRepository
{
    private readonly ILogger<InventoryRepository> _logger;
    private readonly IMongoCollection<Item> _inventoryCollection;
    
    public InventoryRepository(ILogger<InventoryRepository> logger, IOptions<MongoConnectionSettings> settings)
    {
        _logger = logger;
        var mongoClient = new MongoClient($"mongodb://{settings.Value.HostName}:{settings.Value.Port}");
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

        _inventoryCollection = mongoDatabase.GetCollection<Item>("Item");
    }

    public IEnumerable<Item> GetAllItems()
    {
        return _inventoryCollection.Find(_ => true).ToList();
    }

    public Item? GetItemById(string id)
    {
        return _inventoryCollection.Find(_ => _.ItemId == id).FirstOrDefault();
    }

    public void CreateItem(Item item)
    {
        _inventoryCollection.InsertOne(item);
    }

    public void SetReservationOnItem(InventoryRequest request)
    {
        Reservation reservation = new Reservation
        {
            Amount = request.Amount,
            OrderId = request.TransactionId
        };

        _inventoryCollection.UpdateOne(_ => _.ItemId == request.ItemId,
            Builders<Item>.Update.Set(_ => _.PendingReservation, reservation));
    }

    public void ReplaceItem(Item item)
    {
        _inventoryCollection.ReplaceOne(_ => _.ItemId == item.ItemId, item);
    }
}