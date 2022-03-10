using MessageHandling.Abstractions;

namespace EventLibrary;

public class OrderCreated : IMessage
{
    public Guid OrderId;

    public Guid CustomerId;

    public double Total;

    public Dictionary<Guid, int> OrderedItems;
}