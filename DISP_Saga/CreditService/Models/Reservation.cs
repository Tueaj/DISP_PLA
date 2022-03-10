using MongoDB.Bson.Serialization.Attributes;

namespace CreditService.Models;

public class Reservation
{
    [BsonId] public string OrderId;

    public string CustomerId;

    public double Amount;
}