using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace CreditService.Models
{
    public class Credit
    {
        [BsonId]
        public string CreditId;

        public double Amount;

        public CreditLock? Lock;

        public List<CreditChange> ChangeLog;
    }

    public class CreditLock
    {
        public string LockedBy;

        public DateTime LockedAt;
    }
}