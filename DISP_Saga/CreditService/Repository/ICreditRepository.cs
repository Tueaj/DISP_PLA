using System.Collections.Generic;
using CreditService.Models;

namespace CreditService.Repository
{
    public interface ICreditRepository
    {
        IEnumerable<Credit> GetAllCredits();
        public bool CreditExists(string creditId);
        Credit? GetCreditByCreditId(string creditId);
        void CreateCredit(Credit credit);
        void UpdateCredit(Credit credit, string transactionId);
        Credit AcquireCredit(string creditId, string transactionId);
        void ReleaseCredit(string creditId, string transactionId);
        List<Credit> AcquireOldLocks(string transactionId);
    }
}