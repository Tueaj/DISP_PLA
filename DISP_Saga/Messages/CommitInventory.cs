using MessageHandling.Abstractions;

namespace Messages
{
    public class CommitInventory : IMessage
    {
        public string TransactionId;

        public string ItemId;
    }
}