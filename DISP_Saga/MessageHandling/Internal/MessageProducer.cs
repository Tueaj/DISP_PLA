using System;
using System.Collections.Generic;
using System.Text;
using MessageHandling.Abstractions;
using MessageHandling.Internal.Wrappers;
using Newtonsoft.Json;

namespace MessageHandling.Internal
{
    internal class MessageProducer : IMessageProducer
    {
        private readonly RabbitConnection _rabbitConnection;
        private readonly IEnumerable<IMessageProducerInterceptor> _interceptors;
        public MessageProducer(
            RabbitConnection rabbitConnection,
            IEnumerable<IMessageProducerInterceptor> interceptors
        )
        {
            _rabbitConnection = rabbitConnection;
            _interceptors = interceptors;
        }

        public void ProduceMessage(IMessage message, string queue)
        {
            foreach (var messageProducerInterceptor in _interceptors)
            {
                message = messageProducerInterceptor.Intercept(message, queue);
            }
            
            using var channel = _rabbitConnection.Channel;

            var routingKey = message.GetType().FullName!;

            var messageString =
                JsonConvert.SerializeObject(message, ConfigurationConstants.GetJsonSerializerSettings());

            ReadOnlyMemory<byte> messageMemory = new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(messageString));

            channel.BasicPublish(queue, routingKey, false, null, messageMemory);
        }
    }
}