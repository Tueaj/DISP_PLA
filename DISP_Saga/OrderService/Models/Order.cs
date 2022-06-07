using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace OrderService.Models
{
    public class Order
    {
        [BsonId]
        public string OrderId;

        public CreditState Credit;

        public IList<InventoryState> Inventory;

        public OrderStatus Status = OrderStatus.PENDING;
    }

    public enum OrderStatus
    {
        PENDING,
        FAILED,
        COMPLETED
    }
}