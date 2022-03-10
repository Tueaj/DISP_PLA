using MessageHandling.Abstractions;

namespace EventLibrary;

public class InventoryReserved : IMessage
{
    public string OrderId;
}