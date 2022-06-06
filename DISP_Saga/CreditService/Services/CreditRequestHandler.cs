using CreditService.Models;
using MessageHandling;
using MessageHandling.Abstractions;
using Messages;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace CreditService.Services;

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

        Credit? credit = _creditRepository.GetCreditByCustomerId(message.CreditId);

        if (credit == null || credit.Amount < message.Total)
        {
            _producer.ProduceMessage(new CreditReservationFailed() {OrderId = message.OrderId}, QueueName.Command);
            return;
        }

        credit.Amount -= message.Total;

        _creditRepository.UpdateCredit(credit);

        _reservationRepository.CreateReservation(new Reservation
        {
            Amount = message.Total,
            CustomerId = message.CustomerId,
            OrderId = message.OrderId
        });

        _producer.ProduceMessage(new CreditReserved {OrderId = message.OrderId}, QueueName.Command);
    }
}