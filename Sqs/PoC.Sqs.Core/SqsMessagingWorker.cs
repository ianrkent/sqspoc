using PoC.Sqs.Core.Messaging;
using PoC.Sqs.Core.Messaging.Handlers;

namespace PoC.Sqs.Core
{
    public class SqsMessagingWorker
    {
        private readonly IMessageHandler<StockChangeEventV1> _stockChangeHandler;

        public SqsMessagingWorker(IMessageHandler<StockChangeEventV1> stockChangeHandler)
        {
            _stockChangeHandler = stockChangeHandler;
        }

        public void Start()
        {
            _stockChangeHandler.Init();
        }
    }
}

