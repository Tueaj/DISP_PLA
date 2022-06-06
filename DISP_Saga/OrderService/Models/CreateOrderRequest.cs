using System.Collections.Generic;

namespace OrderService.Models
{
    public class CreateOrderRequest
    {
        public string CustomerId;
        public Dictionary<string, int> OrderedItems;
        public double CreditRequired;
    }
}