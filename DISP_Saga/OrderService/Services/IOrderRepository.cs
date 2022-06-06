using System.Collections.Generic;
using OrderService.Models;

namespace OrderService.Services;

public interface IOrderRepository
{
    IEnumerable<CreateOrderRequest> GetAllOrders();
    CreateOrderRequest? GetOrderByOrderId(string orderId);
    void CreateOrder(CreateOrderRequest createOrderRequest);
    void DeleteOrder(string order);
    void CreditReserved(string orderId);
    void InventoryReserved(string orderId);
    bool OrderComplete(string orderId);
}