using MessageHandling.Abstractions;

namespace Messages
{
    public class CommitCredit : IMessage
    {
        public string TransactionId;

        public string CreditId;
    }
}