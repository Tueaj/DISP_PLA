using MessageHandling.Abstractions;

namespace EventLibrary;

public class InventoryReserved : IMessage
{
    public Guid OrderId;
}