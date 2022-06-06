using MessageHandling.Internal.Abstractions;

namespace MessageHandling.Abstractions
{
    /// <summary>
    /// Abstract class for receiving messages sent on the "QUERY" queue.
    /// </summary>
    /// <typeparam name="T">Object type for messages to receive.</typeparam>
    public abstract class QueryHandler<T> : IMessageHandlerImpl<T>
        where T : class, IMessage
    {
        string IMessageHandler.HandlerQueueName => QueueName.Query;

        /// <summary>
        /// <para>
        /// Will be called when a message of the type specified in the class type parameter is sent on the "QUERY" queue.
        /// </para>
        /// <para>
        /// The Handle function will only be called if the RoutingKey of the message matches the fully-qualified name of the type parameter
        /// and is parseable as JSON.
        /// </para>
        /// </summary>
        /// <param name="message">The message received on the "QUERY" queue.</param>
        public abstract void Handle(T message);
    }
}