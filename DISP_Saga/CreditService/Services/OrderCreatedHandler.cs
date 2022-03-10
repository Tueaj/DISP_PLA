using CreditService.Models;
using EventLibrary;
using MessageHandling;
using MessageHandling.Abstractions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace CreditService.Services;

public class OrderCreatedHandler : EventHandler<OrderCreated>
{
    private readonly ILogger<OrderCreatedHandler> _logger;
    private readonly IMessageProducer _producer;
    private readonly ICreditRepository _creditRepository;
    private readonly IReservationRepository _reservationRepository;

    public OrderCreatedHandler(ILogger<OrderCreatedHandler> logger, IMessageProducer producer,
        ICreditRepository creditRepository, IReservationRepository reservationRepository)
    {
        _logger = logger;
        _producer = producer;
        _creditRepository = creditRepository;
        _reservationRepository = reservationRepository;
    }

    public override void Handle(OrderCreated message)
    {
        _logger.LogInformation(message.ToJson());

        Credit? credit = _creditRepository.GetCreditByCustomerId(message.CustomerId);

        if (credit == null || credit.Amount < message.Total)
        {
            // Handle rainy day scenario here
            return;
        }

        credit.Amount -= message.Total;

        _creditRepository.UpdateCredit(credit);

        _reservationRepository.CreateReservation(new Reservation {Amount = message.Total, OrderId = message.OrderId});

        _producer.ProduceMessage(new CreditReserved {OrderId = message.OrderId}, QueueName.Command);
    }
}