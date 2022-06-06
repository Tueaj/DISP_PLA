using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderService.Models;

namespace OrderService.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMongoCollection<Order> _orderCollection;
    
        public OrderRepository(IOptions<MongoConnectionSettings> settings)
        {
            var mongoClient = new MongoClient($"mongodb://{settings.Value.HostName}:{settings.Value.Port}");
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

            _orderCollection = mongoDatabase.GetCollection<Order>("Order");
        }

        public void CreateOrder(Order order)
        {
            _orderCollection.InsertOne(order);
        }

        public Order GetOrderById(string id)
        {
            return _orderCollection.FindSync(_ => _.OrderId == id).First();
        }
    }
}