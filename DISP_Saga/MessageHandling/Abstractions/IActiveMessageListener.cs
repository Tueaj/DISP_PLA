using System;
using System.Runtime.InteropServices;

namespace MessageHandling.Abstractions
{
    public interface IActiveMessageListener<T> : IDisposable where T : class, IMessage
    {
        /// <summary>
        /// Waits for and returns message of type T. Returns immediately if already received after .Listen call.
        /// </summary>
        /// <param name="timeoutDuration">Optionally add timeout to throw after.</param>
        /// <returns>First occurence of message</returns>
        /// <exception cref="TimeoutException">If timeout is declared, throws TimeoutException after duration</exception>
        /// <exception cref="NullReferenceException">If a message is emitted from Rabbit, but no message actually can be parsed, a NullReferenceException is thrown</exception>
        T Get([Optional] TimeSpan? timeoutDuration);
        
        /// <summary>
        /// Waits for and returns message of type T. Returns immediately if already received after .Listen call.
        /// Waits for message with containing specific trace-id.
        /// </summary>
        /// <param name="traceId">Trace-id that should be contained by messages.</param>
        /// <param name="timeoutDuration">Optionally add timeout to throw after.</param>
        /// <returns>First occurence of message matching trace-id</returns>
        /// <exception cref="TimeoutException">If timeout is declared, throws TimeoutException after duration</exception>
        /// <exception cref="NullReferenceException">If a message is emitted from Rabbit, but no message actually can be parsed, a NullReferenceException is thrown</exception>
        T GetTraced(Guid traceId, [Optional] TimeSpan? timeoutDuration);
    }
}