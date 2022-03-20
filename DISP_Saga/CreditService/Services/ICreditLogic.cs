using EventLibrary;

namespace CreditService.Services
{
    public interface ICreditLogic
    {
        public void OrderCreated(OrderCreated message);
        public void OrderFailed(OrderFailed message);
    }
}
