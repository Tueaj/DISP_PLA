using System;
using System.Collections.Generic;
using System.Linq;
using CreditService.Models;
using CreditService.Repository;

namespace CreditService.Services;

public class DummyDataService
{
    private readonly ICreditRepository _creditRepository;

    public DummyDataService(ICreditRepository creditRepository)
    {
        _creditRepository = creditRepository;
    }

    public void CreateDummyData()
    {
        var credits = _creditRepository.GetAllCredits().ToList();
        if (credits.Count is 0)
        {
            return;
        }
        
        Random rnd = new Random();
        for (int i = 0; i < 10; i++)
        {
            _creditRepository.CreateCredit(new Credit
            {
                Amount = rnd.Next(50, 200),
                ChangeLog = new List<CreditChange>(),
                CreditId = Guid.NewGuid().ToString(),
                Lock = null
            });
        }
    }
}