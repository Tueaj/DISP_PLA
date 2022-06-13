using System;
using System.Collections.Generic;
using System.Linq;
using CreditService.Models;
using CreditService.Repository;

namespace CreditService.Services;

public class DummyDataService
{
    public DummyDataService(ICreditRepository creditRepository)
    {
        var credits = creditRepository.GetAllCredits().ToList();
        if (credits.Count is 0)
        {
            return;
        }
        
        Random rnd = new Random();
        for (int i = 0; i < 10; i++)
        {
            creditRepository.CreateCredit(new Credit
            {
                Amount = rnd.Next(50, 200),
                ChangeLog = new List<CreditChange>(),
                CreditId = Guid.NewGuid().ToString(),
                Lock = null
            });
        }
    }
}