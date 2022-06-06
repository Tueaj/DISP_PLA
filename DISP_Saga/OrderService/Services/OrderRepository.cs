using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderService.Models;

namespace OrderService.Services;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<CreateOrderRequest> _orderCollection;
    
    public OrderRepository(IOptions<MongoConnectionSettings> settings)
    {
        var mongoClient = new MongoClient($"mongodb://{settings.Value.HostName}:{settings.Value.Port}");
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

        _orderCollection = mongoDatabase.GetCollection<CreateOrderRequest>("Order");
    }
    
    public IEnumerable<CreateOrderRequest> GetAllOrders()
    {
        return _orderCollection.Find(_ => true).ToList();
    }

    public CreateOrderRequest? GetOrderByOrderId(string orderId)
    {
        return _orderCollection.Find(order => order.OrderId == orderId).FirstOrDefault();
    }

    public void CreateOrder(CreateOrderRequest createOrderRequest)
    {
        _orderCollection.InsertOne(createOrderRequest);
    }

    public void DeleteOrder(string orderId)
    {
        _orderCollection.DeleteOne(order => order.OrderId == orderId);
    }

    public void CreditReserved(string orderId)
    {
        var update = Builders<CreateOrderRequest>.Update.Set(order => order.creditReserved, true);
        _orderCollection.UpdateOneAsync(order => order.OrderId == orderId, update);
    }

    public void InventoryReserved(string orderId)
    {
        var update = Builders<CreateOrderRequest>.Update.Set(order => order.inventoryReserved, true);
        _orderCollection.UpdateOneAsync(order => order.OrderId == orderId, update);
    }

    public bool OrderComplete(string orderId)
    {
        var order = GetOrderByOrderId(orderId);
        return order is {creditReserved: true, inventoryReserved: true};
    }
}