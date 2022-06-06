using MessageHandling.Abstractions;

namespace Messages
{
    public class CommitInventoryAck : IMessage
    {
        public string OrderId;

        public string ItemId;
    }
}