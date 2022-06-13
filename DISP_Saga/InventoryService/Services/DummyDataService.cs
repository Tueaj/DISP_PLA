using System;
using System.Collections.Generic;
using System.Linq;
using InventoryService.Models;
using InventoryService.Repository;

namespace InventoryService.Services;

public class DummyDataService
{
    public DummyDataService(IInventoryRepository inventoryRepository)
    {
        var items = inventoryRepository.GetAllItems().ToList();
        if (items.Count is 0)
        {
            return;
        }
        
        Random rnd = new Random();
        
        for (int i = 0; i < 100; i++)
        {
            inventoryRepository.CreateItem(new Item
            {
                Amount = rnd.Next(10,100),
                ChangeLog = new List<ItemChange>(),
                ItemId = Guid.NewGuid().ToString(),
                Lock = null
            });
        }
    }
}