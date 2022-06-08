﻿using MessageHandling.Abstractions;

namespace Messages
{
    public class CommitInventoryAck : IMessage
    {
        public string TransactionId;

        public string ItemId;
    }
}