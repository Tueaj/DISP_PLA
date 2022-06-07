using MessageHandling.Abstractions;

namespace Messages
{
    public class ShipOrder : IMessage
    {
        public string OrderId;

        public Dictionary<string, int> ItemsToShip;
    }
}