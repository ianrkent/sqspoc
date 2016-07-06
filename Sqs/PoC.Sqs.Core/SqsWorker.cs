using PoC.Sqs.Core.Messages;
using PoC.Sqs.Core.Messages.Handlers;

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

