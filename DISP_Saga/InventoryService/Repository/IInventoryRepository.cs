using System.Collections.Generic;
using InventoryService.Models;

namespace InventoryService.Repository;

public interface IInventoryRepository
{
    IEnumerable<Item> GetAllItems();
    public bool ItemExists(string itemId);
    Item? GetItemById(string itemId);
    void CreateItem(Item item);
    void UpdateItem(Item item, string transactionId);
    Item AcquireItem(string itemId, string transactionId);
    void ReleaseItem(string itemId, string transactionId);
    List<Item> AcquireOldLocks(string transactionId);
}