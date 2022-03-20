using EventLibrary;

namespace InventoryService.Services
{
    public interface IInventoryLogic
    {
        void OrderCreated(OrderCreated message);
        void OrderFailed(OrderFailed message);
    }
}
