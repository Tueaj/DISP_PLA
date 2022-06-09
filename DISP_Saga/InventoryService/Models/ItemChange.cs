namespace InventoryService.Models
{
    public class ItemChange
    {
        public string TransactionId;

        public int Amount;

        public ItemChangeStatus Status;
    }
    
    public enum ItemChangeStatus
    {
        Pending,
        Performed,
        RolledBack,
        Aborted
    }
}