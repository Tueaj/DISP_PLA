using MessageHandling.Abstractions;

namespace Messages
{
    public class InventoryRequestNack : IMessage
    {
        public string ItemId;

        public string TransactionId;
    }
}