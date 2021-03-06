using System;
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
            IMongoClient mongoClient;

            if (settings.Value.Credentials == null)
            {
                mongoClient = new MongoClient($"mongodb://{settings.Value.HostName}:{settings.Value.Port}");
            }
            else
            {
                mongoClient =
                    new MongoClient(
                        $"mongodb://{settings.Value.Credentials.Username}:{settings.Value.Credentials.Password}@{settings.Value.HostName}:{settings.Value.Port}");
            }
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

            _orderCollection = mongoDatabase.GetCollection<Order>("Order");
        }

        public void CreateOrder(Order order)
        {
            order.TransactionId = Guid.NewGuid().ToString();
            _orderCollection.InsertOne(order);
        }

        public Order GetOrderById(string id)
        {
            return _orderCollection.FindSync(_ => _.TransactionId == id).First();
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _orderCollection.Find(_ => true).ToList();
        }

        public void UpdateOrder(Order order)
        {
            _orderCollection.FindOneAndReplace(_ => _.TransactionId == order.TransactionId, order);
        }
    }
}