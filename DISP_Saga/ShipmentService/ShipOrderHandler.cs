using System.Linq;
using MessageHandling;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;

namespace ShipmentService
{
    class ShipOrderHandler : CommandHandler<ShipOrder>
    {
        private readonly ILogger<ShipOrderHandler> _logger;
        private readonly IMessageProducer _messageProducer;

        public ShipOrderHandler(ILogger<ShipOrderHandler> logger, IMessageProducer messageProducer)
        {
            _logger = logger;
            _messageProducer = messageProducer;
        }

        public override void Handle(ShipOrder message)
        {
            _logger.LogInformation($"Order succeeded with id: {message.TransactionId} and items : {message.ItemsToShip.Select(item => item.Key)}");
            _messageProducer.ProduceMessage(new ShipOrderAck()
            {
                TransactionId = message.TransactionId
            }, QueueName.Command);
        }
    }
}