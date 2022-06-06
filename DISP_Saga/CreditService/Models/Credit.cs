using MongoDB.Bson.Serialization.Attributes;

namespace CreditService.Models;

public class Credit
{
    [BsonId] public string CreditId;

    public double Amount;

    public Reservation? PendingReservation;
}