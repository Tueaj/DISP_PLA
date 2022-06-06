using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MessageHandling.Abstractions;
using MessageHandling.Internal.Wrappers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageHandling.Internal
{
    /// <summary>
    /// Internal HostedService for creating connections between any registered <see cref="IMessageHandler"/> and RabbitMQ exchanges.
    /// </summary>
    internal class MessageProvider : IHostedService
    {
        private readonly ILogger<MessageProvider> _logger;
        private readonly IEnumerable<IMessageHandler> _messageHandlers;
        private readonly RabbitConnection _rabbitConnection;

        public MessageProvider(
            ILogger<MessageProvider> logger,
            IEnumerable<IMessageHandler> messageHandlers,
            RabbitConnection rabbitConnection
        )
        {
            _logger = logger;
            _messageHandlers = messageHandlers;
            _rabbitConnection = rabbitConnection;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var channel = _rabbitConnection.Channel;

            foreach (var messageHandler in _messageHandlers)
            {
                string handlerName = messageHandler.GetType().FullName!;
                channel.QueueDeclare(handlerName, true, false, false);
                channel.QueueBind(handlerName, messageHandler.HandlerQueueName, messageHandler.MessageType.FullName);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (ch, ea) =>
                {
                    try
                    {
                        var body = System.Text.Encoding.Default.GetString(ea.Body.ToArray());

                        messageHandler.HandleDelegate((
                            JsonConvert.DeserializeObject(body, messageHandler.MessageType, ConfigurationConstants.GetJsonSerializerSettings()) as IMessage)!);

                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Failed to handle message with Routing Key: {}, sent on Exchange: {}", ea.RoutingKey, ea.Exchange);
                        channel.BasicNack(ea.DeliveryTag, false, false);
                    }
                };

                channel.BasicConsume(handlerName, false, consumer);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}