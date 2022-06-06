using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using OrderService.Models;

namespace OrderService.Services
{
    public interface IOrderLogic
    {
        ActionResult<IEnumerable<Order>> GetAllOrders();
        ActionResult<Order> GetOrderById(string id);
        ActionResult<Order> CreateOrder(Order order);
    }

}
