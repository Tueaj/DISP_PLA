using System;

namespace MessageHandling.Abstractions
{
    public interface IMessageHandler
    {
        internal Type MessageType { get; }

        internal delegate void HandlerMethod(IMessage message);

        internal HandlerMethod HandleDelegate { get; }

        internal string HandlerQueueName { get; }
    }
}