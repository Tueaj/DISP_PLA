using CreditService.Models;
using Microsoft.Extensions.Logging;

namespace CreditService.Services;

public class OrderFailedHandler : EventHandler<OrderFailed>
{
    private readonly ILogger<OrderFailedHandler> _logger;
    private readonly IReservationRepository _reservationRepository;
    private readonly ICreditRepository _creditRepository;

    public OrderFailedHandler(ILogger<OrderFailedHandler> logger, IReservationRepository reservationRepository, ICreditRepository creditRepository)
    {
        _logger = logger;
        _reservationRepository = reservationRepository;
        _creditRepository = creditRepository;
    }

    public override void Handle(OrderFailed message)
    {
        _logger.LogInformation(message.ToJson());

        Reservation? reservation = _reservationRepository.GetReservation(message.OrderId);

        if (reservation == null)
        {
            return;
        }

        Credit? credit = _creditRepository.GetCreditByCustomerId(reservation.CustomerId);

        if (credit != null)
        {
            credit.Amount += reservation.Amount;
            _creditRepository.UpdateCredit(credit);
        }
        
        _reservationRepository.DeleteReservation(reservation.OrderId);
    }
}