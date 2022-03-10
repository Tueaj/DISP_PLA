using MessageHandling.Abstractions;

namespace EventLibrary;

public class InventoryReservedFailed : IMessage
{
    public string OrderId;
}