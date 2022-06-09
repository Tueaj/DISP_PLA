using System.Collections.Generic;
using OrderService.Models;

namespace OrderService.Services
{
    public interface IOrderRepository
    {
        void CreateOrder(Order order);

        Order GetOrderById(string id);

        IEnumerable<Order> GetAllOrders();

        void UpdateOrder(Order order);
    }
}