using System.Collections.Generic;
using CreditService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CreditService.Services;

public class CreditRepository : ICreditRepository
{
    private readonly ILogger<CreditRepository> _logger;
    private readonly IMongoCollection<Credit> _creditCollection;
    
    public CreditRepository(ILogger<CreditRepository> logger, IOptions<MongoConnectionSettings> settings)
    {
        _logger = logger;
        var mongoClient = new MongoClient($"mongodb://{settings.Value.HostName}:{settings.Value.Port}");
        var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

        _creditCollection = mongoDatabase.GetCollection<Credit>("Credit");
    }

    public IEnumerable<Credit> GetAllCredits()
    {
        return _creditCollection.Find(_ => true).ToList();
    }

    public Credit? GetCreditByCustomerId(string customerId)
    {
        return _creditCollection.Find(_ => _.CustomerId == customerId).FirstOrDefault();
    }

    public void CreateCredit(Credit credit)
    {
        _creditCollection.InsertOne(credit);
    }

    public void UpdateCredit(Credit credit)
    {
        _creditCollection.UpdateOne(_ => _.CustomerId == credit.CustomerId, Builders<Credit>.Update.Set(_ => _.Amount, credit.Amount));
    }
}