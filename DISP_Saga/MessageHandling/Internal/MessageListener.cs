using System;
using MessageHandling.Abstractions;
using MessageHandling.Internal.Wrappers;
using Microsoft.Extensions.Logging;

namespace MessageHandling.Internal
{
    internal class MessageListener : IMessageListener
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitConnection _rabbitConnection;
        
        public MessageListener(
            IServiceProvider serviceProvider,
            RabbitConnection rabbitConnection
        )
        {
            _serviceProvider = serviceProvider;
            _rabbitConnection = rabbitConnection;
        }

        public IActiveMessageListener<T> Listen<T>(string queue)
            where T : class, IMessage
        {
            return new ActiveMessageListener<T>(
                _serviceProvider.GetService(typeof(ILogger<IActiveMessageListener<T>>)) as
                    ILogger<IActiveMessageListener<T>>,
                _rabbitConnection.Channel, queue);
        }
    }
}