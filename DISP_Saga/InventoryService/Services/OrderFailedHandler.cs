using InventoryService.Models;
using Microsoft.Extensions.Logging;

namespace InventoryService.Services;

public class OrderFailedHandler : EventHandler<OrderFailed>
{
    private readonly ILogger<OrderFailedHandler> _logger;
    private readonly IReservationRepository _reservationRepository;
    private readonly IInventoryRepository _inventoryRepository;

    public OrderFailedHandler(ILogger<OrderFailedHandler> logger, IReservationRepository reservationRepository, IInventoryRepository inventoryRepository)
    {
        _logger = logger;
        _reservationRepository = reservationRepository;
        _inventoryRepository = inventoryRepository;
    }

    public override void Handle(OrderFailed message)
    {
        _logger.LogInformation(message.ToJson());

        Reservation? reservation = _reservationRepository.GetReservation(message.OrderId);

        if (reservation == null)
        {
            return;
        }

        foreach (var reservedItem in reservation.ItemReservations)
        {
            Item? item = _inventoryRepository.GetItemByName(reservedItem.Name);

            if (item == default)
            {
                continue;
            }

            item.Amount += reservedItem.Amount;
            
            _inventoryRepository.UpdateItem(item);
        }
        
        _reservationRepository.DeleteReservation(reservation.OrderId);
    }
}