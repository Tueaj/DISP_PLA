using MessageHandling.Abstractions;

namespace EventLibrary;

public class InventoryReservationFailed : IMessage
{
    public string OrderId;
}