namespace MessageHandling.Abstractions
{
    public interface IMessageProducer
    {
        /// <summary>
        /// Post a message to a specific queue.
        /// </summary>
        /// <param name="message">The message to place in the queue.</param>
        /// <param name="queue">The specific queue to place the message in. Should be one of the queues in <see cref="QueueName"/>.</param>
        void ProduceMessage(IMessage message, string queue);
    }
}