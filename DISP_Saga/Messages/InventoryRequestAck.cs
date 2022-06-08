using MessageHandling.Abstractions;

namespace Messages
{
    public class InventoryRequestAck : IMessage
    {
        public string ItemId;

        public string TransactionId;
    }
}