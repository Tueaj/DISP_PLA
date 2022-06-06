using MessageHandling.Abstractions;

namespace Messages
{
    public class InventoryRequest : IMessage
    {
        public int Amount;

        public string ItemId;

        public string OrderId;
    }
}