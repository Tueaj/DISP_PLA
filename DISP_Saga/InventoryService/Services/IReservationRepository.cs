using System.Collections.Generic;
using InventoryService.Models;

namespace InventoryService.Services;

public interface IReservationRepository
{
    void CreateReservation(Reservation reservation);

    IEnumerable<Reservation> GetReservations();
    
    Reservation? GetReservation(string orderId);
    
    void DeleteReservation(string orderId);
}