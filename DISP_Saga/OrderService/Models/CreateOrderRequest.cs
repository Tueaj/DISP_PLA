using System.Collections.Generic;

namespace OrderService.Models
{
    public class CreateOrderRequest
    {
        public string CreditId;
        public Dictionary<string, int> OrderedItems;
        public double CreditRequired;
    }
}