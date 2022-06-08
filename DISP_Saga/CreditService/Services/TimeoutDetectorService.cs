using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CreditService.Models;
using CreditService.Repository;
using Microsoft.Extensions.Hosting;

namespace CreditService.Services
{
    public class TimeoutDetectorService : BackgroundService
    {
        private readonly ICreditRepository _repository;

        public TimeoutDetectorService(
            ICreditRepository repository
        )
        {
            _repository = repository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                var transactionId = Guid.NewGuid();
                var lockedCredits = _repository.AcquireOldLocks(transactionId.ToString());

                foreach (var lockedCredit in lockedCredits)
                {
                    var pendingChanges = lockedCredit.ChangeLog
                        .Where(log => log.Status == CreditChangeStatus.Pending);

                    foreach (var pendingChange in pendingChanges)
                    {
                        pendingChange.Status = CreditChangeStatus.Aborted;
                    }

                    _repository.UpdateCredit(lockedCredit, transactionId.ToString());
                    _repository.ReleaseCredit(lockedCredit.CreditId, transactionId.ToString());
                }
            }
        }
    }
}