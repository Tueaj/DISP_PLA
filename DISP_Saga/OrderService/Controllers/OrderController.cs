using System.Collections.Generic;
using System.Linq;
using MessageHandling;
using MessageHandling.Abstractions;
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
        public IEnumerable<Order> GetOrders()
        {
            return _orderRepository.GetAllOrders();
        }

        [HttpPost("")]
        public ActionResult<CreateOrderRequest> CreateOrder(CreateOrderRequest createOrderRequest)
        {
            var order = new Order
            {
                Credit = new CreditState
                {
                    Amount = createOrderRequest.CreditRequired,
                    CreditId = createOrderRequest.CreditId
                },
                Inventory = createOrderRequest.OrderedItems.Select(pair => new InventoryState
                {
                    ItemId = pair.Key,
                    Amount = pair.Value
                }).ToList()
            };

            _orderRepository.CreateOrder(order);

            _messageProducer.ProduceMessage(new CreditRequest
            {
                TransactionId = order.TransactionId,
                CreditId = order.Credit.CreditId,
                Amount = order.Credit.Amount
            }, QueueName.Command);

            foreach (var inventoryState in order.Inventory)
            {
                _messageProducer.ProduceMessage(new InventoryRequest
                {
                    TransactionId = order.TransactionId,
                    ItemId = inventoryState.ItemId,
                    Amount = inventoryState.Amount,
                }, QueueName.Command);
            }

            return Created("Order creation begun", createOrderRequest);
        }
    }
}