namespace MessageHandling
{
    /// <summary>
    /// Constants for names of the various queues available in MessageHandling
    /// </summary>
    public static class QueueName
    {
        /// <summary>
        /// The Query queue is intended for sending messages to retrieve data.
        /// Responses containing the queried data should be sent on the Response queue.
        /// </summary>
        public const string Query = "QUERY";
        /// <summary>
        /// The Command queue is intended for mutational operations in the system.
        /// Responses containing information about the completed command should be sent on the response queue
        /// </summary>
        public const string Command = "COMMAND";
        /// <summary>
        /// The response queue is intended for responses to queries or commands.
        /// </summary>
        public const string Response = "RESPONSE";
    }
}