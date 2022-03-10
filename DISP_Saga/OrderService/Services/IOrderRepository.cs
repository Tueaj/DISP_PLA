using OrderService.Models;

namespace OrderService.Services;

public interface IOrderRepository
{
    IEnumerable<Order> GetAllOrders();
    Order? GetOrderByOrderId(Guid orderId);
    void CreateOrder(Order order);
    void DeleteOrder(Guid order);
}