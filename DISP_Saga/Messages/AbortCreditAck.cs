using MessageHandling.Abstractions;

namespace Messages
{
    public class AbortCreditAck : IMessage
    {
        public string TransactionId;

        public string CreditId;
    }
}