using System;

namespace MessageHandling.Abstractions
{
    /// <summary>
    /// Message that contains an ID for tracing responses to specific messages.
    /// If a response has the trace-id for a specific command or query, it is
    /// assumed to be a response for that command or query.
    /// </summary>
    public interface ITracedMessage : IMessage
    {
        /// <summary>
        /// The trace-id to correlate responses to commands or queries.
        /// </summary>
        public Guid TraceID { get; set; }
    }
}