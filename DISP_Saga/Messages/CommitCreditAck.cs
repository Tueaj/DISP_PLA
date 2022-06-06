using MessageHandling.Abstractions;

namespace Messages
{
    public class CommitCreditAck : IMessage
    {
        public string OrderId;

        public string CreditId;
    }
}