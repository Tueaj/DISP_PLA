using System;
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
        private readonly IOrderLogic _orderLogic;

        public OrderController(IOrderLogic orderLogic)
        {
            _orderLogic = orderLogic;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Order>> GetOrder()
        {
            try
            {
                return _orderLogic.GetAllOrders();
            }
            catch (Exception exception)
            {
                //Log error
                Console.WriteLine(exception.Message);
                throw;
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Order> GetOrder(string id)
        {
            try
            {
                return _orderLogic.GetOrderById(id);
            }
            catch (Exception exception)
            {
                //Log error
                Console.WriteLine(exception.Message);
                throw;
            }
           
        }

        [HttpPost("")]
        public ActionResult<Order> CreateOrder(Order order)
        {
            try
            {
                return _orderLogic.CreateOrder(order);
            }
            catch (Exception exception)
            {
                //Log error
                Console.WriteLine(exception.Message);
                throw;
            }
        }
    }
}