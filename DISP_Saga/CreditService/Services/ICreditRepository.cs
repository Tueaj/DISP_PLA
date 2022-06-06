using System.Collections.Generic;
using CreditService.Models;

namespace CreditService.Services;

public interface ICreditRepository
{
    IEnumerable<Credit> GetAllCredits();
    Credit? GetCreditByCustomerId(string creditId);
    void CreateCredit(Credit credit);
    void UpdateCredit(Credit credit);
}