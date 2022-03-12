using System.Collections.Generic;
using InventoryService.Models;

namespace InventoryService.Services;

public interface IInventoryRepository
{
    IEnumerable<Item> GetAllItems();
    Item? GetItemByName(string name);
    void CreateItem(Item item);
    void UpdateItem(Item item);
}