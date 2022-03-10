using System.Collections.Generic;
using OrderService.Models;

namespace OrderService.Services;

public interface IOrderRepository
{
    IEnumerable<Order> GetAllOrders();
    Order? GetOrderByOrderId(string orderId);
    void CreateOrder(Order order);
    void DeleteOrder(string order);
    void CreditReserved(string orderId);
    void InventoryReserved(string orderId);
    bool OrderComplete(string orderId);
}