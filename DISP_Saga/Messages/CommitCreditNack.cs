using MessageHandling.Abstractions;

namespace Messages
{
    public class CommitCreditNack : IMessage
    {
        public string TransactionId;

        public string CreditId;
    }
}