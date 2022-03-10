using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CreditService.Models;

public class Credit
{
    public Credit()
    {
        
    }

    public Credit(string customerId, double amount)
    {
        CustomerId = customerId;
        Amount = amount;
    }
    
    [BsonId]
    public string CustomerId;

    public double Amount;
}