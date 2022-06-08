using MessageHandling.Abstractions;

namespace Messages
{
    public class CommitInventoryNack : IMessage
    {
        public string TransactionId;

        public string ItemId;
    }
}