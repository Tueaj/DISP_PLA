using OrderService.Models;

namespace OrderService.Services
{
    public interface IOrderRepository
    { 
        void CreateOrder(Order order);

        Order GetOrderById(string id);

        void UpdateOrder(Order order);
    }
}