using MessageHandling.Abstractions;

namespace Messages
{
    public class RollbackCreditAck : IMessage
    {
        public string TransactionId;

        public string CreditId;
    }
}