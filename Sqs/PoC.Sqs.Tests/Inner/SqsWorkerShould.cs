using NSubstitute;
using NUnit.Framework;
using PoC.Sqs.Core;
using PoC.Sqs.Core.Messaging;
using PoC.Sqs.Core.Messaging.Handlers;

namespace PoC.Sqs.Tests.Inner
{
    [TestFixture]
    public class SqsWorkerShould : AutoSubstituteTestBase<SqsWorker>
    {
        private SqsWorker TheSqsWorker => Sut;

        [Test]
        public void IntialiseTheStockEventChangeHandler()
        {
            TheSqsWorker.Start();
            TheDependency<IMessageHandler<StockChangeEventV1>>().Received(1).Init();
        }
    }
}