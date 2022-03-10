using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;

namespace CreditService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public IEnumerable<Order> GetCredits()
        {
            return _orderRepository.GetAllOrders();
        }

        [HttpGet("{id}")]
        public ActionResult<Order> GetCredit(Guid id)
        {
            var foundOrder = _orderRepository.GetOrderByOrderId(id);
            
            if (foundOrder == null)
            {
                return NotFound();
            }

            return Ok(foundOrder);
        }

        [HttpPost("")]
        public ActionResult<Order> CreateCredit(Order order)
        {
            var foundOrder = _orderRepository.GetOrderByOrderId(order.OrderId);

            if (foundOrder == null)
            {
                _orderRepository.CreateOrder(order);
            }
            else
            {
                return BadRequest();
            }

            return Created("Order succesfull", order);
        }
    }
}