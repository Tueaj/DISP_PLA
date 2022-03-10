﻿using MongoDB.Bson.Serialization.Attributes;

namespace CreditService.Models;

public class Credit
{
    [BsonId] public string CustomerId;

    public double Amount;
}