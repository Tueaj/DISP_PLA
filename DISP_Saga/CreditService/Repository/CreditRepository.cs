using System;
using System.Collections.Generic;
using System.Data;
using CreditService.Models;
using CreditService.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CreditService.Repository
{
    public class CreditRepository : ICreditRepository
    {
        private readonly IMongoCollection<Credit> _creditCollection;

        public CreditRepository(IOptions<MongoConnectionSettings> settings)
        {
            IMongoClient mongoClient;

            if (settings.Value.Credentials == null)
            {
                mongoClient = new MongoClient($"mongodb://{settings.Value.HostName}:{settings.Value.Port}");
            }
            else
            {
                mongoClient =
                    new MongoClient(
                        $"mongodb://{settings.Value.Credentials.Username}:{settings.Value.Credentials.Password}@{settings.Value.HostName}:{settings.Value.Port}");
            }
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);

            _creditCollection = mongoDatabase.GetCollection<Credit>("Credit");

            var dummyDataService = new DummyDataService(this);
            dummyDataService.CreateDummyData();
        }

        public IEnumerable<Credit> GetAllCredits()
        {
            return _creditCollection.Find(_ => true).ToList();
        }

        public Credit? GetCreditByCreditId(string creditId)
        {
            return _creditCollection.Find(_ => _.CreditId == creditId).FirstOrDefault();
        }

        public bool CreditExists(string creditId)
        {
            return _creditCollection.CountDocuments(_ => _.CreditId == creditId) != 0;
        }

        public void CreateCredit(Credit credit)
        {
            _creditCollection.InsertOne(credit);
        }

        public void UpdateCredit(Credit credit, string transactionId)
        {
            Credit? updated = null;
            try
            {
                updated = _creditCollection.FindOneAndReplace(
                    _ => _.CreditId == credit.CreditId && (_.Lock == null || _.Lock.LockedBy == transactionId), credit);
            }
            catch (MongoException e) when (e is MongoWriteConcernException or MongoWriteException)
            {
            }

            if (updated == null)
            {
                throw new ConstraintException($"Did not hold required lock for credit with id {credit.CreditId}");
            }
        }

        public Credit AcquireCredit(string creditId, string transactionId)
        {
            Credit? credit = null;
            try
            {
                credit = _creditCollection.FindOneAndUpdate(
                    _ => _.CreditId == creditId && (_.Lock == null || _.Lock.LockedBy == transactionId),
                    Builders<Credit>.Update.Set(_ => _.Lock,
                        new CreditLock {LockedAt = DateTime.Now, LockedBy = transactionId}));
            }
            catch (MongoException e) when (e is MongoWriteConcernException or MongoWriteException)
            {
            }

            if (credit == null)
            {
                throw new ConstraintException($"Could not acquire lock for credit with ID {creditId}");
            }

            return credit;
        }

        public void ReleaseCredit(string creditId, string transactionId)
        {
            Credit? credit = null;
            try
            {
                credit = _creditCollection.FindOneAndUpdate(
                    _ => _.CreditId == creditId && (_.Lock == null || _.Lock.LockedBy == transactionId),
                    Builders<Credit>.Update.Set(_ => _.Lock, null));
            }
            catch (MongoException e) when (e is MongoWriteConcernException or MongoWriteException)
            {
            }

            if (credit == null)
            {
                throw new ConstraintException($"Could not release lock for credit with ID {creditId}");
            }
        }

        public List<Credit> AcquireOldLocks(string transactionId)
        {
            List<Credit> expiredCredits = _creditCollection
                .FindSync(_ => _.Lock != null && _.Lock.LockedAt.CompareTo(DateTime.Now.AddMinutes(-1)) >= 0).ToList();

            List<Credit> lockedCredits = new List<Credit>();
            foreach (var expiredCredit in expiredCredits)
            {
                Credit? tryLock = null;
                try
                {
                    tryLock = _creditCollection.FindOneAndUpdate(
                        _ => _.CreditId == expiredCredit.CreditId && _.Lock != null &&
                             _.Lock.LockedAt.CompareTo(DateTime.Now.AddMinutes(-1)) >= 0,
                        Builders<Credit>.Update.Set(_ => _.Lock, new CreditLock
                        {
                            LockedAt = DateTime.Now,
                            LockedBy = transactionId
                        }));
                }
                catch (MongoException e) when (e is MongoWriteConcernException or MongoWriteException)
                {
                }

                if (tryLock != null)
                {
                    lockedCredits.Add(tryLock);
                }
            }

            return lockedCredits;
        }
    }
}