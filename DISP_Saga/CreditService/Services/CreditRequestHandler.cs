using System;
using CreditService.Models;
using CreditService.Repository;
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

        Credit? credit = _creditRepository.GetCreditByCreditId(message.CreditId);

        if (credit == null || credit.Amount < message.Amount)
        {
            //_producer.ProduceMessage(new CreditReservationFailed() {OrderId = message.OrderId}, QueueName.Command);
            return;
        }

        if (credit.PendingReservation is not null)
        {
            throw new Exception(); //fix type of exception
        }

        var reservation = new Reservation
        {
            OrderId = message.OrderId,
            Amount = message.Amount
        };

        _creditRepository.AddReservation(message.CreditId, reservation);

        _producer.ProduceMessage(new CreditRequestAck {OrderId = message.OrderId, CreditId = message.CreditId}, QueueName.Command);
    }
}