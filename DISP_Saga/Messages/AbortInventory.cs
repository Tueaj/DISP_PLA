using MessageHandling.Abstractions;

namespace Messages
{
    public class AbortInventory : IMessage
    {
        public string TransactionId;

        public string ItemId;
    }
}