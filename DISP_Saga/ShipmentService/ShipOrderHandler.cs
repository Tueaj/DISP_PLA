using System.Linq;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;

namespace ShipmentService
{
    class ShipOrderHandler : CommandHandler<ShipOrder>
    {
        private readonly ILogger<ShipOrderHandler> _logger;

        public ShipOrderHandler(ILogger<ShipOrderHandler> logger)
        {
            _logger = logger;
        }

        public override void Handle(ShipOrder message)
        {
            _logger.LogInformation($"Order succeeded with id: {message.OrderId} and items : {message.ItemsToShip.Select(item => item.Key)}");
        }
    }
}