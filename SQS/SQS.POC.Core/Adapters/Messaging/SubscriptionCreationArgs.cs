using System;
using System.Threading.Tasks;
using SQS.POC.Core.Messages;

namespace SQS.POC.Core.Adapters.Messaging
{
    public class SubscriptionCreationArgs
    {
        public Type ContractType { get; set; }
        public Func<StockChangeEventV1, Task> MessageHandler { get; set; }
        public string Filter { get; set; }
    }
}