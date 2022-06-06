using MessageHandling.Abstractions;

namespace EventLibrary
{
    public class CreditReservationFailed : IMessage
    {
        public string OrderId;
    }
}