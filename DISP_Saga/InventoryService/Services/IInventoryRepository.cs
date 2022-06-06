using System.Collections.Generic;
using InventoryService.Models;
using Messages;

namespace InventoryService.Services;

public interface IInventoryRepository
{
    IEnumerable<Item> GetAllItems();
    Item? GetItemById(string id);
    void SetReservationOnItem(InventoryRequest request);
    void CreateItem(Item item);
    void ReplaceItem(Item item);
}