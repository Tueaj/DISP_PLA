using System;
using System.Collections.Generic;
using InventoryService.Models;
using InventoryService.Repository;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryController(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        [HttpGet]
        public IEnumerable<Item> GetInventory()
        {
            return _inventoryRepository.GetAllItems();
        }

        [HttpGet("{id}")]
        public ActionResult<Item> GetItem(string id)
        {
            Item? foundItem = _inventoryRepository.GetItemById(id);

            if (foundItem == null)
            {
                return NotFound();
            }

            return Ok(foundItem);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateItem([FromRoute] string id, [FromBody] int amount)
        {
            Item? item = _inventoryRepository.GetItemById(id);

            if (item == null)
            {
                _inventoryRepository.CreateItem(new Item {Amount = amount, ItemId = id, ChangeLog = new List<ItemChange>(), Lock = null});
            }
            else
            {
                item.Amount = amount;
                _inventoryRepository.UpdateItem(item, Guid.NewGuid().ToString());
            }

            return Ok();
        }
    }
}