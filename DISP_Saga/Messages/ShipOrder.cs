using MessageHandling.Abstractions;

namespace Messages
{
    public class ShipOrder : IMessage
    {
        public string TransactionId;

        public Dictionary<string, int> ItemsToShip;
    }
}