using System;
using System.Collections.Generic;
using CreditService.Models;
using CreditService.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CreditService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditController : ControllerBase
    {
        private readonly ICreditRepository _creditRepository;

        public CreditController(ICreditRepository creditRepository)
        {
            _creditRepository = creditRepository;
        }

        [HttpGet]
        public IEnumerable<Credit> GetCredits()
        {
            return _creditRepository.GetAllCredits();
        }

        [HttpGet("{id}")]
        public ActionResult<Credit> GetCredit(string id)
        {
            Credit? foundCredit = _creditRepository.GetCreditByCreditId(id);

            if (foundCredit == null)
            {
                return NotFound();
            }

            return Ok(foundCredit);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCredit([FromRoute] string id, [FromBody] double credit)
        {
            Credit? foundCredit = _creditRepository.GetCreditByCreditId(id);

            if (foundCredit == null)
            {
                _creditRepository.CreateCredit(new Credit
                {
                    Amount = credit, 
                    CreditId = id,
                    ChangeLog = new List<CreditChange>(),
                    Lock = null
                });
            }
            else
            {
                foundCredit.Amount = credit;
                _creditRepository.UpdateCredit(foundCredit, Guid.NewGuid().ToString());
            }

            return Ok();
        }
    }
}