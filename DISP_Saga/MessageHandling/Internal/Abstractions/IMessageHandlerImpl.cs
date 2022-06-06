using System;
using MessageHandling.Abstractions;

namespace MessageHandling.Internal.Abstractions
{
    interface IMessageHandlerImpl<T> : IMessageHandler
        where T : class, IMessage
    {
        public void Handle(T message);

        private void HandleInternal(IMessage message)
        {
            Handle((message as T)!);
        }

        Type IMessageHandler.MessageType => typeof(T);

        IMessageHandler.HandlerMethod IMessageHandler.HandleDelegate => HandleInternal;
    }
}