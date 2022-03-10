using MongoDB.Bson.Serialization.Attributes;

namespace CreditService.Models;

public class Reservation
{
    [BsonId] public string OrderId;

    public double Amount;
}