namespace EventLibrary;

public class OrderCreated
{
    public Guid OrderId;

    public double Total;

    public Dictionary<Guid, int> OrderedItems;
}