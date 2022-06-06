using MongoDB.Bson.Serialization.Attributes;

namespace InventoryService.Models;

public class Item
{
    [BsonId]
    public string ItemId;

    public string Name;

    public int Amount;
    
    public Reservation? PendingReservation;
}