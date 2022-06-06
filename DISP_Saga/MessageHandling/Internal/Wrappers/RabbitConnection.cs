using System;
using System.Threading;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MessageHandling.Internal.Wrappers
{
    internal class RabbitConnection
    {
        private const int amountOfRetrys = 10;
        private static readonly TimeSpan retryInterval = TimeSpan.FromSeconds(10);

        public IModel Channel => GetChannel();

        private IConnection _connection;

        private readonly IOptions<MessageHandlingConfiguration> _options;

        public RabbitConnection(IOptions<MessageHandlingConfiguration> options)
        {
            _options = options;
        }

        private IModel GetChannel()
        {
            IModel channel = default;
            for (int i = 0; i <= amountOfRetrys; i++)
            {
                try
                {
                    if (_connection == default)
                    {
                        CreateConnection();

                        using var declarer = _connection.CreateModel();
                        
                        declarer.ExchangeDeclare(QueueName.Command, ExchangeType.Direct, true);
                        declarer.ExchangeDeclare(QueueName.Query, ExchangeType.Direct, true);
                        declarer.ExchangeDeclare(QueueName.Response, ExchangeType.Direct, true);
                    }

                    channel = _connection.CreateModel();
                    break;
                }
                catch (Exception)
                {
                    if (amountOfRetrys == i)
                    {
                        throw;
                    }

                    Thread.Sleep(retryInterval);
                }
            }
            
            return channel;
        }

        private void CreateConnection()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = _options.Value.HostName,
                Port = _options.Value.Port,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _connection = connectionFactory.CreateConnection();
        }
    }
}