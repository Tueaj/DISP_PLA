using CreditService.Models;
using CreditService.Repository;
using MessageHandling;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace CreditService.Services
{
    public class CommitCreditHandler : CommandHandler<CommitCredit>
    {
        private readonly ILogger<CommitCreditHandler> _logger;
        private readonly IMessageProducer _producer;
        private readonly ICreditRepository _creditRepository;

        public CommitCreditHandler(
            ILogger<CommitCreditHandler> logger,
            IMessageProducer producer,
            ICreditRepository creditRepository)
        {
            _logger = logger;
            _producer = producer;
            _creditRepository = creditRepository;
        }

        public override void Handle(CommitCredit message)
        {
            _logger.LogInformation(message.ToJson());

            // Ignore the constraint exception - If we fail to acquire the lock, we should just throw, re-queue,
            // and try again.
            Credit credit = _creditRepository.AcquireCredit(message.CreditId, message.TransactionId);

            var change = credit.ChangeLog.Find(change => change.TransactionId == message.TransactionId);

            if (change is not {Status: CreditChangeStatus.Pending})
            {
                if (change?.Status == CreditChangeStatus.Aborted)
                {
                    _producer.ProduceMessage(new CommitCreditNack
                        {
                            TransactionId = message.TransactionId,
                            CreditId = message.CreditId
                        },
                        QueueName.Command);
                }
                else
                {
                    _producer.ProduceMessage(
                        new CommitCreditAck {TransactionId = message.TransactionId, CreditId = message.CreditId},
                        QueueName.Command);
                }

                _creditRepository.ReleaseCredit(message.CreditId, message.TransactionId);

                return;
            }

            credit.Amount += change.Amount;
            change.Status = CreditChangeStatus.Performed;

            _creditRepository.UpdateCredit(credit, message.TransactionId);
            _creditRepository.ReleaseCredit(message.CreditId, message.TransactionId);

            _producer.ProduceMessage(
                new CommitCreditAck {TransactionId = message.TransactionId, CreditId = message.CreditId},
                QueueName.Command);
        }
    }
}