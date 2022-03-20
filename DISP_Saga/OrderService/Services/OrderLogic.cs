using EventLibrary;
using MessageHandling;
using MessageHandling.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using OrderService.Models;
using MongoDB.Driver;

namespace OrderService.Services
{
    public class OrderLogic : IOrderLogic
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageProducer _messageProducer;

        public OrderLogic(IOrderRepository orderRepository, IMessageProducer messageProducer)
        {
            _orderRepository = orderRepository;
            _messageProducer = messageProducer;
        }

        public ActionResult<IEnumerable<Order>> GetAllOrders()
        {
            return new OkObjectResult(_orderRepository.GetAllOrders());
        }

        public ActionResult<Order> GetOrderById(string id)
        {
            //So far no need to catch mongodb exceptions - will be caught and logged in outermost catch clause in controller
            Order? foundOrder = _orderRepository.GetOrderByOrderId(id);
            
            if (foundOrder == null)
            {
                return new NotFoundObjectResult("Order was not found");
            }

            return new OkObjectResult(foundOrder);
        }

        public ActionResult<Order> CreateOrder(Order order)
        {
            //So far no need to catch mongodb exceptions - will be caught and logged in outermost catch clause in controller
            var foundOrder = _orderRepository.GetOrderByOrderId(order.OrderId);

            if (foundOrder == null)
            {
                order.creditReserved = false;
                order.inventoryReserved = false;
                _orderRepository.CreateOrder(order);
            }
            else
            {
                return new ConflictObjectResult("CONFLICT: Order already exists.");
            }

            _messageProducer.ProduceMessage(new OrderCreated
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                Total = order.Total,
                OrderedItems = order.OrderedItems
            }, QueueName.Command);

            return new CreatedResult("Order created", order);
        }
    }
}
