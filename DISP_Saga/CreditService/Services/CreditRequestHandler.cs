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
    public class CreditRequestHandler : CommandHandler<CreditRequest>
    {
        private readonly ILogger<CreditRequestHandler> _logger;
        private readonly IMessageProducer _producer;
        private readonly ICreditRepository _creditRepository;

        public CreditRequestHandler(
            ILogger<CreditRequestHandler> logger,
            IMessageProducer producer,
            ICreditRepository creditRepository)
        {
            _logger = logger;
            _producer = producer;
            _creditRepository = creditRepository;
        }

        public override void Handle(CreditRequest message)
        {
            _logger.LogInformation(message.ToJson());

            if (!_creditRepository.CreditExists(message.CreditId))
            {
                _producer.ProduceMessage(new CreditRequestNack
                {
                    CreditId = message.CreditId,
                    TransactionId = message.TransactionId
                }, QueueName.Command);
                return;
            }

            // Ignore the constraint exception - If we fail to acquire the lock, we should just throw, re-queue,
            // and try again.
            Credit credit = _creditRepository.AcquireCredit(message.CreditId, message.TransactionId);

            var existingChange = credit.ChangeLog.FirstOrDefault(log => log.TransactionId == message.TransactionId);

            if (existingChange != default)
            {
                if (existingChange.Status == CreditChangeStatus.Aborted)
                {
                    _producer.ProduceMessage(new CreditRequestNack
                    {
                        CreditId = message.CreditId,
                        TransactionId = message.TransactionId
                    }, QueueName.Command);
                }
                else
                {
                    _producer.ProduceMessage(
                        new CreditRequestAck {TransactionId = message.TransactionId, CreditId = message.CreditId},
                        QueueName.Command);
                }

                if (existingChange.Status != CreditChangeStatus.Pending)
                {
                    _creditRepository.ReleaseCredit(message.CreditId, message.TransactionId);
                }

                return;
            }
            
            var creditChange = new CreditChange
            {
                Amount = message.Amount,
                Status = CreditChangeStatus.Pending,
                TransactionId = message.TransactionId
            };

            credit.ChangeLog.Add(creditChange);

            _creditRepository.UpdateCredit(credit, message.TransactionId);
            
            if (credit.Amount < message.Amount)
            {
                _creditRepository.ReleaseCredit(message.CreditId, message.TransactionId);
                _producer.ProduceMessage(new CreditRequestNack
                {
                    CreditId = message.CreditId,
                    TransactionId = message.TransactionId
                }, QueueName.Command);
                return;
            }

            // Don't release lock on credit, we need to keep it until the commit is completed

            _producer.ProduceMessage(
                new CreditRequestAck {TransactionId = message.TransactionId, CreditId = message.CreditId},
                QueueName.Command);
        }
    }
}