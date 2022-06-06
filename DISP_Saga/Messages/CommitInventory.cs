using MessageHandling.Abstractions;

namespace Messages
{
    public class CommitInventory : IMessage
    {
        public string OrderId;

        public string ItemId;
    }
}