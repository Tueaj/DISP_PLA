using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessageHandling.Abstractions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace MessageHandling.Internal
{
    internal class ActiveMessageListener<T> : IActiveMessageListener<T> where T: class, IMessage
    {
        private readonly ILogger<IActiveMessageListener<T>> _logger;
        private readonly EventWaitHandle _waitHandle = new (false, EventResetMode.AutoReset);
        private readonly Guid _guid = Guid.NewGuid();
        private readonly EventingBasicConsumer _consumer;
        private readonly List<IMessage> _receivedMessages = new();

        public ActiveMessageListener(ILogger<IActiveMessageListener<T>> logger, IModel channel, string queue)
        {
            _logger = logger;
            
            channel.QueueDeclare(_guid.ToString(), false, true, true, null);
            channel.QueueBind(_guid.ToString(), queue, typeof(T).FullName);
            
            _consumer = new EventingBasicConsumer(channel);
            _consumer.Received += (ch, ea) =>
            {
                try
                {
                    var body = System.Text.Encoding.Default.GetString(ea.Body.ToArray());

                    _receivedMessages.Add(JsonConvert.DeserializeObject(body, typeof(T),
                        ConfigurationConstants.GetJsonSerializerSettings()) as IMessage);

                    _waitHandle.Set();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Failed to handle message with Routing Key: {}, sent on Exchange: {}",
                        ea.RoutingKey, ea.Exchange);
                }
            };
            
            channel.BasicConsume(_consumer, _guid.ToString(), true);
        }

        public T Get(TimeSpan? timeoutDuration = null)
        {
            if (_receivedMessages.Count > 0)
            {
                var foundMessage = _receivedMessages.First() as T;
                _receivedMessages.RemoveAt(0);
                return foundMessage;
            }

            if (timeoutDuration != null)
            {
                if (!_waitHandle.WaitOne(timeoutDuration.Value))
                {
                    throw new TimeoutException();
                }
            }
            else
            {
                _waitHandle.WaitOne();
            }
            
            if (_receivedMessages.Count == 0)
            {
                throw new NullReferenceException();
            }
            
            var message = _receivedMessages.First() as T;
            _receivedMessages.RemoveAt(0);
            return message;
        }

        public T GetTraced(Guid traceId, TimeSpan? timeoutDuration = null)
        {
            Task delay = Task.Delay(TimeSpan.FromMilliseconds(-1));
            if (timeoutDuration != null)
            {
                delay = Task.Delay(timeoutDuration.Value);
            }

            Task<T> listenTask = Task.Run(() =>
            {
                ITracedMessage message = default;

                while (message == default)
                {
                    message = (ITracedMessage)_receivedMessages.Find(m => (m as ITracedMessage).TraceID == traceId);
                    _receivedMessages.Remove(message);

                    if (message == default)
                    {
                        _waitHandle.WaitOne();
                    }
                }

                return message as T;
            });

            Task.WhenAny(delay, listenTask).Wait();

            if (listenTask.IsCompletedSuccessfully)
            {
                return listenTask.Result;
            }

            throw new TimeoutException();
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _waitHandle?.Dispose();
                foreach (var tag in _consumer.ConsumerTags)
                {
                    try
                    {
                        _consumer.Model.BasicCancel(tag);
                    }
                    catch (OperationInterruptedException){}
                }
                _consumer.Model.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        ~ActiveMessageListener()
        {
            Dispose(disposing: false);
        }
    }
}