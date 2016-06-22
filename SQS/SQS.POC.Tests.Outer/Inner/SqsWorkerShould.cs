using NSubstitute;
using NUnit.Framework;
using SQS.POC.Core;
using SQS.POC.Core.Messages;

namespace SQS.POC.Tests.Inner
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