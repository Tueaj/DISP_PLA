using System.Collections.Generic;
using CreditService.Models;

namespace CreditService.Repository;

public interface ICreditRepository
{
    IEnumerable<Credit> GetAllCredits();
    Credit? GetCreditByCreditId(string creditId);
    void CreateCredit(Credit credit);
    void UpdateCredit(Credit credit);
    void AddReservation(string creditId, Reservation reservation);
}