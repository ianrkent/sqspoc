using NSubstitute;
using NUnit.Framework;
using PoC.Sqs.Core;
using PoC.Sqs.Core.Messaging;
using PoC.Sqs.Core.Messaging.Handlers;

namespace PoC.Sqs.Tests.Inner
{
    [TestFixture]
    public class SqsWorkerShould : AutoSubstituteTestBase<SqsMessagingWorker>
    {
        private SqsMessagingWorker _theSqsMessagingWorker => Sut;

        [Test]
        public void IntialiseTheStockEventChangeHandler()
        {
            _theSqsMessagingWorker.Start();
            TheDependency<IMessageHandler<StockChangeEventV1>>().Received(1).Init();
        }
    }
}