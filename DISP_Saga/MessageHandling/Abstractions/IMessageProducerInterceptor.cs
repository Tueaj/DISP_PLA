namespace MessageHandling.Abstractions
{
    public interface IMessageProducerInterceptor
    {
        IMessage Intercept(IMessage message, string queue);
    }
}