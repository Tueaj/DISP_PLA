using CreditService.Models;
using EventLibrary;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace CreditService.Services
{
    public class CreditLogic : ICreditLogic
    {
        private readonly ILogger<CreditLogic> _logger;
        private readonly ICreditRepository _creditRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly MongoClient _mongoClient;


        public CreditLogic(ILogger<CreditLogic> logger, ICreditRepository creditRepository, IReservationRepository reservationRepository, 
            IOptions<MongoConnectionSettings> settings)
        {
            _logger = logger;
            _creditRepository = creditRepository;
            _reservationRepository = reservationRepository;
            _mongoClient = new MongoClient($"mongodb://{settings.Value.HostName}:{settings.Value.Port}");
        }

        public void OrderCreated(OrderCreated message)
        {
            _logger.LogInformation(message.ToJson());

            Credit? credit = _creditRepository.GetCreditByCustomerId(message.CustomerId);

            if (credit == null || credit.Amount < message.Total)
            {
                //Should throw custom exception that can be checked for in handlers catch clause
                throw new Exception("Not enough credit");
            }

            credit.Amount -= message.Total;

            Reservation reservation = new Reservation
            {
                Amount = message.Total,
                CustomerId = message.CustomerId,
                OrderId = message.OrderId
            };

            using(var transaction =  _mongoClient.StartSession())
            {
                transaction.StartTransaction();

                try
                {
                    _creditRepository.UpdateCredit(credit);
                    _reservationRepository.CreateReservation(reservation);
                    transaction.CommitTransaction();
                }
                catch (Exception)
                {
                     transaction.AbortTransaction();
                    throw;
                }
            }
        }

        public void OrderFailed(OrderFailed message)
        {
            _logger.LogInformation(message.ToJson());

            Reservation? reservation = _reservationRepository.GetReservation(message.OrderId);

            if (reservation == null)
            {
                return;
            }

            Credit? credit = _creditRepository.GetCreditByCustomerId(reservation.CustomerId);

            if (credit == null)
            {
                return;
            }

            credit.Amount += reservation.Amount;

            using (var transaction = _mongoClient.StartSession())
            {
                transaction.StartTransaction();

                try
                {
                    _creditRepository.UpdateCredit(credit);
                    _reservationRepository.DeleteReservation(reservation.OrderId);
                    transaction.CommitTransaction();
                }
                catch (Exception)
                {
                    transaction.AbortTransaction();
                    throw;
                }
            }
        }
    }

}
