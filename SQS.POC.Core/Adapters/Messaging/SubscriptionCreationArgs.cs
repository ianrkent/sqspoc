using System;
using SQS.POC.Core.Messages;

namespace SQS.POC.Core.Adapters.Messaging
{
    public class SubscriptionCreationArgs
    {
        public Type ContractType { get; set; }
        public Action<StockChangeEventV1> MessageHandler { get; set; }
        public string Filter { get; set; }
    }
}