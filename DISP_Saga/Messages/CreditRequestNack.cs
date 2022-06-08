using MessageHandling.Abstractions;

namespace Messages
{
    public class CreditRequestNack : IMessage
    {
        public string CreditId;

        public string TransactionId;
    }
}