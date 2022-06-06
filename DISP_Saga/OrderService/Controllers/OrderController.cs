using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMessageProducer _messageProducer;

        public OrderController(IOrderRepository orderRepository, IMessageProducer messageProducer)
        {
            _orderRepository = orderRepository;
            _messageProducer = messageProducer;
        }

        [HttpGet]
        public IEnumerable<Order> GetOrder()
        {
            return _orderRepository.GetAllOrders();
        }

        [HttpGet("{id}")]
        public ActionResult<Order> GetOrder(string id)
        {
            var foundOrder = _orderRepository.GetOrderByOrderId(id);
            
            if (foundOrder == null)
            {
                return NotFound();
            }

            return Ok(foundOrder);
        }

        [HttpPost("")]
        public ActionResult<Order> CreateOrder(Order order)
        {
            var foundOrder = _orderRepository.GetOrderByOrderId(order.OrderId);
            order.creditReserved = false;
            order.inventoryReserved = false;
            
            if (foundOrder == null)
            {
                _orderRepository.CreateOrder(order);
            }
            else
            {
                return Conflict();
            }

            _messageProducer.ProduceMessage(new OrderCreated
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                Total =  order.Total,
                OrderedItems = order.OrderedItems
            }, QueueName.Command);
            
            return Created("Order created", order);
        }
    }
}