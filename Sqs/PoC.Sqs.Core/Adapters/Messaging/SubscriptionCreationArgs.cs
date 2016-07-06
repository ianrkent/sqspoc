using System;
using System.Threading.Tasks;
using PoC.Sqs.Core.Messages;

namespace PoC.Sqs.Core.Adapters.Messaging
{
    public class SubscriptionCreationArgs
    {
        public Type ContractType { get; set; }
        public Func<StockChangeEventV1, Task> MessageHandler { get; set; }
        public string Filter { get; set; }
    }
}