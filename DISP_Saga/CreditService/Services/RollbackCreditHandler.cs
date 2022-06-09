using System.Linq;
using CreditService.Models;
using CreditService.Repository;
using MessageHandling;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace CreditService.Services
{
    public class RollbackCreditHandler : CommandHandler<RollbackCredit>
    {
        private readonly ILogger<RollbackCreditHandler> _logger;
        private readonly IMessageProducer _producer;
        private readonly ICreditRepository _creditRepository;

        public RollbackCreditHandler(
            ILogger<RollbackCreditHandler> logger,
            IMessageProducer producer,
            ICreditRepository creditRepository)
        {
            _logger = logger;
            _producer = producer;
            _creditRepository = creditRepository;
        }

        public override void Handle(RollbackCredit message)
        {
            _logger.LogInformation(message.ToJson());

            if (_creditRepository.CreditExists(message.CreditId))
            {
                // Ignore the constraint exception - If we fail to acquire the lock, we should just throw, re-queue,
                // and try again.
                Credit credit = _creditRepository.AcquireCredit(message.CreditId, message.TransactionId);

                var change = credit.ChangeLog.FirstOrDefault(log => log.TransactionId == message.TransactionId);

                if (change != default)
                {
                    if (change.Status == CreditChangeStatus.Performed)
                    {
                        credit.Amount += change.Amount;
                        change.Status = CreditChangeStatus.RolledBack;
                    }

                    if (change.Status == CreditChangeStatus.Pending)
                    {
                        change.Status = CreditChangeStatus.RolledBack;
                    }

                    _creditRepository.UpdateCredit(credit, message.TransactionId);
                }

                _creditRepository.ReleaseCredit(credit.CreditId, message.TransactionId);
            }
            _producer.ProduceMessage(
                new RollbackCreditAck {TransactionId = message.TransactionId, CreditId = message.CreditId},
                QueueName.Command);
        }
    }
}