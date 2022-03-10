namespace EventLibrary;

public class OrderSucceeded
{
    public Guid OrderId;
    
    public Dictionary<Guid, int> OrderedItems;
}