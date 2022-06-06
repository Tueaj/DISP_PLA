using System.Collections.Generic;
using CreditService.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CreditService.Repository;

public class CreditRepository : ICreditRepository
{
    private readonly IMongoCollection<Credit> _creditCollection;
    
    public CreditRepository(IOptions<MongoConnectionSettings> settings)
    {
        var mongoClient = new MongoClient($"mongodb://{settings.Value.HostName}:{settings.Value.Port}");
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

        _creditCollection = mongoDatabase.GetCollection<Credit>("Credit");
    }

    public IEnumerable<Credit> GetAllCredits()
    {
        return _creditCollection.Find(_ => true).ToList();
    }

    public Credit? GetCreditByCreditId(string creditId)
    {
        return _creditCollection.Find(_ => _.CreditId == creditId).FirstOrDefault();
    }

    public void CreateCredit(Credit credit)
    {
        _creditCollection.InsertOne(credit);
    }

    public void UpdateCredit(Credit credit)
    {
        _creditCollection.UpdateOne(_ => _.CreditId == credit.CreditId, Builders<Credit>.Update.Set(_ => _.Amount, credit.Amount));
    }

    public void AddReservation(string creditId, Reservation reservation)
    {
        _creditCollection.UpdateOne(_ => _.CreditId == creditId, Builders<Credit>.Update.Set(_ => _.PendingReservation, reservation));
    }
}