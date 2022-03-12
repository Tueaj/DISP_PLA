﻿using System.Collections.Generic;
using InventoryService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace InventoryService.Services;

public class ReservationRepository : IReservationRepository
{
    private readonly ILogger<ReservationRepository> _logger;
    private readonly IMongoCollection<Reservation> _reservationCollection;
    
    public ReservationRepository(ILogger<ReservationRepository> logger, IOptions<MongoConnectionSettings> settings)
    {
        _logger = logger;
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

        _reservationCollection = mongoDatabase.GetCollection<Reservation>("Reservation");
    }
    
    public void CreateReservation(Reservation reservation)
    {
        _reservationCollection.InsertOne(reservation);
    }

    public IEnumerable<Reservation> GetReservations()
    {
        return _reservationCollection.Find(_ => true).ToList();
    }

    public Reservation? GetReservation(string orderId)
    {
        return _reservationCollection.Find(_ => _.OrderId == orderId).FirstOrDefault();
    }

    public void DeleteReservation(string orderId)
    {
        _reservationCollection.DeleteOne(_ => _.OrderId == orderId);
    }
}