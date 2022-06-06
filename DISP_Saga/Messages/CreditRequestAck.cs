using MessageHandling.Abstractions;

namespace Messages
{
    public class CreditRequestAck : IMessage
    {
        public string CreditId;

        public string OrderId;
    }
}