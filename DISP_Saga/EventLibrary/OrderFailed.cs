using MessageHandling.Abstractions;

namespace EventLibrary;

public class OrderFailed : IMessage
{
    public string OrderId;
}