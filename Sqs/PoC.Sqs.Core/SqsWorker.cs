using PoC.Sqs.Core.Messaging;
using PoC.Sqs.Core.Messaging.Handlers;

namespace PoC.Sqs.Core
{
    public class SqsWorker
    {
        private readonly IMessageHandler<StockChangeEventV1> _stockChangeHandler;

        public SqsWorker(IMessageHandler<StockChangeEventV1> stockChangeHandler)
        {
            _stockChangeHandler = stockChangeHandler;
        }

        public void Start()
        {
            _stockChangeHandler.Init();
        }
    }
}

