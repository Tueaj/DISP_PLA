using MessageHandling.Abstractions;

namespace EventLibrary;

public class OrderSucceeded : IMessage
{
    public Guid OrderId;
    
    public Dictionary<Guid, int> OrderedItems;
}