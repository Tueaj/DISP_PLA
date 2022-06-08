using MessageHandling.Abstractions;

namespace Messages
{
    public class RollbackCredit : IMessage
    {
        public string TransactionId;

        public string CreditId;
    }
}