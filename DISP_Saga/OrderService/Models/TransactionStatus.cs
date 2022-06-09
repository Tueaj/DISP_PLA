namespace OrderService.Models
{
    public enum TransactionStatus
    {
        Pending,
        Requested,
        Committed,
        Aborted,
        Rolledback
    }
}