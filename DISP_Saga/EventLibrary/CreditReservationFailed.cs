using MessageHandling.Abstractions;

namespace EventLibrary;

public class CreditReservedFailed : IMessage
{
    public string OrderId;
}