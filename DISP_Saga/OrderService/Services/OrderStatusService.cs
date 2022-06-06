using System;

namespace OrderService.Services
{
    public class OrderStatusService
    {
        private event EventHandler<OrderStatusUpdatedEventArgs> OrderStatusUpdated;

        public OrderStatusService()
        {
            
        }

        public void OrderUpdated(string orderId)
        {
            OrderStatusUpdated(this, new OrderStatusUpdatedEventArgs
            {
                OrderId = orderId
            });
        }
    }

    public class OrderStatusUpdatedEventArgs : EventArgs
    {
        public string OrderId;
    }
}