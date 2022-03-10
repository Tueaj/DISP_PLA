using CreditService.Models;

namespace CreditService.Services;

public interface ICreditRepository
{
    IEnumerable<Credit> GetAllCredits();
    Credit? GetCreditByCustomerId(string customerId);
    void CreateCredit(Credit credit);
    void UpdateCredit(Credit credit);
}