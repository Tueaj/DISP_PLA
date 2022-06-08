using MessageHandling.Abstractions;

namespace Messages
{
    public class CommitCreditAck : IMessage
    {
        public string TransactionId;

        public string CreditId;
    }
}