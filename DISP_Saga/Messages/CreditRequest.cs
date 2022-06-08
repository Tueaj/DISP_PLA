using MessageHandling.Abstractions;

namespace Messages
{
    public class CreditRequest : IMessage
    {
        public double Amount;

        public string CreditId;

        public string TransactionId;
    }
}