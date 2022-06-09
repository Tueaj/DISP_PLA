using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace InventoryService.Models;

public class Item
{
    [BsonId]
    public string ItemId;

    public string Name;

    public int Amount;

    public ItemLock? Lock;
    
    public List<ItemChange> ChangeLog;
}

public class ItemLock
{
    public string LockedBy;

    public DateTime LockedAt;
}