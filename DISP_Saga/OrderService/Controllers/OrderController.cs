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
        private readonly OrderStatusService _orderStatusService;

        public OrderController(
            IOrderRepository orderRepository,
            OrderStatusService orderStatusService)
        {
            _orderRepository = orderRepository;
            _orderStatusService = orderStatusService;
            ReplayPendingEvent();
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

           _orderStatusService.OrderUpdated(order.TransactionId);

            return Created("Order creation begun", createOrderRequest);
        }

        private void ReplayPendingEvent()
        {
            var nonCompletedOrders = _orderRepository.GetAllOrders().ToList()
                .FindAll(order => order.Status == OrderStatus.PENDING);

            foreach (var order in nonCompletedOrders)
            {
                _orderStatusService.OrderUpdated(order.TransactionId);
            }
        }
    }
}