using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderService.Models;

namespace OrderService.Services;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _orderCollection;
    
    public OrderRepository(IOptions<MongoConnectionSettings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

        _orderCollection = mongoDatabase.GetCollection<Order>("Order");
    }
    
    public IEnumerable<Order> GetAllOrders()
    {
        return _orderCollection.Find(_ => true).ToList();
    }

    public Order? GetOrderByOrderId(string orderId)
    {
        return _orderCollection.Find(order => order.OrderId == orderId).FirstOrDefault();
    }

    public void CreateOrder(Order order)
    {
        _orderCollection.InsertOne(order);
    }

    public void DeleteOrder(string orderId)
    {
        _orderCollection.DeleteOne(order => order.OrderId == orderId);
    }

    public void CreditReserved(string orderId)
    {
        var update = Builders<Order>.Update.Set(order => order.creditReserved, true);
        _orderCollection.UpdateOneAsync(order => order.OrderId == orderId, update);
    }

    public void InventoryReserved(string orderId)
    {
        var update = Builders<Order>.Update.Set(order => order.inventoryReserved, true);
        _orderCollection.UpdateOneAsync(order => order.OrderId == orderId, update);
    }

    public bool OrderComplete(string orderId)
    {
        var order = GetOrderByOrderId(orderId);
        return order.creditReserved && order.inventoryReserved;
    }
}