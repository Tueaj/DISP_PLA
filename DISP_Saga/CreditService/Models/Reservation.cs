using MongoDB.Bson.Serialization.Attributes;

namespace CreditService.Models;

public class Reservation
{
    public string OrderId;

    public double Amount;
}