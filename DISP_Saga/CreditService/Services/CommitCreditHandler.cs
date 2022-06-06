using CreditService.Models;
using CreditService.Repository;
using MessageHandling;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace CreditService.Services;

public class CommitCreditHandler : CommandHandler<CommitCredit>
{
    private readonly ILogger<CreditRequestHandler> _logger;
    private readonly IMessageProducer _producer;
    private readonly ICreditRepository _creditRepository;

    public CommitCreditHandler(
        ILogger<CreditRequestHandler> logger, 
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

        Credit credit = _creditRepository.GetCreditByCreditId(message.CreditId);
        
        credit.Amount -= credit.PendingReservation.Amount;
        credit.PendingReservation = null;
        
        _creditRepository.UpdateCredit(credit);

        _producer.ProduceMessage(new CommitCreditAck {OrderId = message.OrderId, CreditId = message.CreditId}, QueueName.Command);
    }
}