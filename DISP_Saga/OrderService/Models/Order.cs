using MongoDB.Bson.Serialization.Attributes;

namespace OrderService.Models;

public class Order
{
    [BsonId]
    public Guid OrderId;
    
    public string CustomerId;
    
    public Dictionary<Guid, int> OrderedItems;

    public double Total;
}