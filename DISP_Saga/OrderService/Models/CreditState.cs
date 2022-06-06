namespace OrderService.Models
{
    public class CreditState
    {
        public double Amount;

        public string CreditId;
        
        public TransactionStatus Status = TransactionStatus.PENDING;
    }
}