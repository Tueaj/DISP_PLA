using MessageHandling.Abstractions;

namespace Messages
{
    public class ShipOrderAck : IMessage
    {
        public string OrderId;
    }
}