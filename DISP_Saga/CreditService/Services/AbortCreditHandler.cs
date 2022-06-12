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
    public class AbortCreditHandler : CommandHandler<AbortCredit>
    {
        private readonly ILogger<AbortCreditHandler> _logger;
        private readonly IMessageProducer _producer;
        private readonly ICreditRepository _creditRepository;

        public AbortCreditHandler(
            ILogger<AbortCreditHandler> logger,
            IMessageProducer producer,
            ICreditRepository creditRepository)
        {
            _logger = logger;
            _producer = producer;
            _creditRepository = creditRepository;
        }

        public override void Handle(AbortCredit message)
        {
            _logger.LogInformation(message.ToJson());

            if (_creditRepository.CreditExists(message.CreditId))
            {
                // Ignore the constraint exception - If we fail to acquire the lock, we should just throw, re-queue,
                // and try again.
                Credit credit = _creditRepository.AcquireCredit(message.CreditId, message.TransactionId);

                var creditChangeLog = credit.ChangeLog.Find(change =>
                    change.TransactionId == message.TransactionId);
                
                if (creditChangeLog != default)
                {
                    creditChangeLog.Status = CreditChangeStatus.Aborted;
                    _creditRepository.UpdateCredit(credit, message.TransactionId);
                }


                _creditRepository.ReleaseCredit(credit.CreditId, message.TransactionId);
            }
            _producer.ProduceMessage(
                new AbortCreditAck {TransactionId = message.TransactionId, CreditId = message.CreditId},
                QueueName.Command);
        }
    }
}