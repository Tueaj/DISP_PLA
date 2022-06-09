using MessageHandling.Abstractions;

namespace Messages
{
    public class AbortCredit : IMessage
    {
        public string TransactionId;

        public string CreditId;
    }
}