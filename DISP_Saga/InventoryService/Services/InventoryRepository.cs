using System.Collections.Generic;
using InventoryService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InventoryService.Services;

public class InventoryRepository : IInventoryRepository
{
    private readonly ILogger<InventoryRepository> _logger;
    private readonly IMongoCollection<Item> _creditCollection;
    
    public InventoryRepository(ILogger<InventoryRepository> logger, IOptions<MongoConnectionSettings> settings)
    {
        _logger = logger;
        var mongoClient = new MongoClient($"mongodb://{settings.Value.HostName}:{settings.Value.Port}");
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

        _creditCollection = mongoDatabase.GetCollection<Item>("Item");
    }

    public IEnumerable<Item> GetAllItems()
    {
        return _creditCollection.Find(_ => true).ToList();
    }

    public Item? GetItemByName(string name)
    {
        return _creditCollection.Find(_ => _.Name == name).FirstOrDefault();
    }

    public void CreateItem(Item item)
    {
        _creditCollection.InsertOne(item);
    }

    public void UpdateItem(Item item)
    {
        _creditCollection.UpdateOne(_ => _.Name == item.Name, Builders<Item>.Update.Set(_ => _.Amount, item.Amount));
    }
}