using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderService.Models;

public class Order
{
    [BsonId]
    public string OrderId;
    public string CustomerId;
    public Dictionary<string, int> OrderedItems;
    public double Total;
    public OrderState orderState;
}

public enum OrderState
{
    PENDING,
    CREDIT_RESERVED,
    INVENTORY_RESERVED,
    FAILED,
    COMPLETE
}