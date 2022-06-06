using System.Collections.Generic;
using MessageHandling;
using Messages;
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
        public IEnumerable<CreateOrderRequest> GetOrder()
        {
            return _orderRepository.GetAllOrders();
        }

        [HttpGet("{id}")]
        public ActionResult<CreateOrderRequest> GetOrder(string id)
        {
            var foundOrder = _orderRepository.GetOrderByOrderId(id);

            if (foundOrder == null)
            {
                return NotFound();
            }

            return Ok(foundOrder);
        }

        [HttpPost("")]
        public ActionResult<CreateOrderRequest> CreateOrder(CreateOrderRequest createOrderRequest)
        {
            _orderRepository.CreateOrder(createOrderRequest);

            _messageProducer.ProduceMessage(new CreditRequest
            {
                OrderId = createOrderRequest.OrderId,
                CustomerId = createOrderRequest.CustomerId,
                Total = createOrderRequest.Total,
                OrderedItems = createOrderRequest.OrderedItems
            }, QueueName.Command);
            
            _messageProducer.ProduceMessage(new InventoryRequest
            {
                
            })

            return Created("Order created", createOrderRequest);
        }
    }
}