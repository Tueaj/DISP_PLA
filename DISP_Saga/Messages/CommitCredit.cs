using MessageHandling.Abstractions;

namespace Messages
{
    public class CommitCredit : IMessage
    {
        public string OrderId;

        public string CreditId;
    }
}