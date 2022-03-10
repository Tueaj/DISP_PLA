namespace EventLibrary;

public class OrderCreated
{
    public Guid OrderId;

    public Guid CustomerId;

    public double Total;

    public Dictionary<Guid, int> OrderedItems;
}