using MessageHandling.Abstractions;

namespace EventLibrary;

public class OrderCreated : IMessage
{
    public string OrderId;

    public string CustomerId;

    public double Total;

    public Dictionary<string, int> OrderedItems;
}