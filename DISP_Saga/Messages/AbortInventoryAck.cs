using MessageHandling.Abstractions;

namespace Messages
{
    public class AbortInventoryAck : IMessage
    {
        public string TransactionId;

        public string ItemId;
    }
}