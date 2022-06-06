namespace MessageHandling.Abstractions
{
    /// <summary>
    /// API for synchronously and asynchronously retrieving an occurrence of a IMessage.
    /// </summary>
    public interface IMessageListener
    {
        /// <summary>
        /// Begins listening on specified queue. If message of type T is sent on queue, it will be available for later
        /// calls to .Get
        /// </summary>
        /// <param name="queue">Message queue to listen for occurrences of message on.</param>
        /// <typeparam name="T">Type of message to listen for.</typeparam>
        /// <returns>Guid to identify listener for later Get calls</returns>
        IActiveMessageListener<T> Listen<T>(string queue) where T : class, IMessage;

    }
}