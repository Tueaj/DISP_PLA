using System.Collections.Generic;
using CreditService.Models;

namespace CreditService.Services;

public interface IReservationRepository
{
    void CreateReservation(Reservation reservation);

    IEnumerable<Reservation> GetReservations();
    
    Reservation? GetReservation(string orderId);
    
    void DeleteReservation(string orderId);
}