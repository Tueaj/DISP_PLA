using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace InventoryService.Models;

public class Reservation
{
    [BsonId] public string OrderId;

    public List<Item> ItemReservations = new();
}