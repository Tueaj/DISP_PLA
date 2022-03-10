using MessageHandling.Abstractions;

namespace EventLibrary;

public abstract class EventHandler<T> : CommandHandler<T>
    where T : class, IMessage
{
}