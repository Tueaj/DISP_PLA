namespace OrderService.Models
{
    public enum TransactionStatus
    {
        Pending,
        Requested,
        Committed,
        Aborted,
        Abort,
        Rolledback,
        Rollback
    }
}