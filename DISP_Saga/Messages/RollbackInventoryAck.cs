using MessageHandling.Abstractions;

namespace Messages
{
    public class RollbackInventoryAck : IMessage
    {
        public string TransactionId;

        public string ItemId;
    }
}