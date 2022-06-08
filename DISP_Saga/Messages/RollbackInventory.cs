using MessageHandling.Abstractions;

namespace Messages
{
    public class RollbackInventory : IMessage
    {
        public string TransactionId;

        public string ItemId;
    }
}