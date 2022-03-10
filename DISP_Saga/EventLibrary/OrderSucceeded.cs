using MessageHandling.Abstractions;

namespace EventLibrary;

public class OrderSucceeded : IMessage
{
    public string OrderId;
    
    public Dictionary<string, int> OrderedItems;
}