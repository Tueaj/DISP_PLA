using Microsoft.Extensions.Logging;

namespace CreditService.Services;

public class OrderSucceededHandler : EventHandler<OrderSucceeded>
{
    private readonly ILogger<OrderSucceededHandler> _logger;
    private readonly IReservationRepository _repository;
    
    public OrderSucceededHandler(ILogger<OrderSucceededHandler> logger, IReservationRepository reservationRepository)
    {
        _logger = logger;
        _repository = reservationRepository;
    }
    
    public override void Handle(OrderSucceeded message)
    {
        _logger.LogInformation(message.ToJson());
        
        _repository.DeleteReservation(message.OrderId);
    }
}