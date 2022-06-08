namespace CreditService.Models
{
    public class CreditChange
    {
        public string TransactionId;

        public double Amount;

        public CreditChangeStatus Status;
    }

    public enum CreditChangeStatus
    {
        Pending,
        Performed,
        RolledBack,
        Aborted
    }
}