using MessageHandling.Abstractions;

namespace EventLibrary;

public class CreditReserved : IMessage
{
    public Guid OrderId;
}