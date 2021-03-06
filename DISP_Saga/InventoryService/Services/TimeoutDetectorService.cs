using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InventoryService.Models;
using InventoryService.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InventoryService.Services
{
    public class TimeoutDetectorService : BackgroundService
    {
        private readonly IInventoryRepository _repository;
        private readonly ILogger<TimeoutDetectorService> _logger;

        public TimeoutDetectorService(
            IInventoryRepository repository, 
            ILogger<TimeoutDetectorService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                var transactionId = Guid.NewGuid();
                
                var lockedItems = _repository.AcquireOldLocks(transactionId.ToString());
                foreach (var lockedItem in lockedItems)
                {
                    var pendingChanges = lockedItem.ChangeLog
                        .Where(log => log.Status == ItemChangeStatus.Pending);

                    foreach (var pendingChange in pendingChanges)
                    {
                        pendingChange.Status = ItemChangeStatus.Aborted;
                    }

                    _repository.UpdateItem(lockedItem, transactionId.ToString());
                    _repository.ReleaseItem(lockedItem.ItemId, transactionId.ToString());
                }
            }
        }
    }
}