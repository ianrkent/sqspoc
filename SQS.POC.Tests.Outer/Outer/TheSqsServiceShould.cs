using NSubstitute;
using NUnit.Framework;
using SQS.POC.Core;
using SQS.POC.Core.Adapters.Azure;
using SQS.POC.Core.Adapters.Configuration;
using SQS.POC.Core.Messages;

namespace SQS.POC.Tests.Outer
{
    [TestFixture]
    public class TheSqsServiceShould : AutoSubstituteFixture<SqsWorker>
    {
        [Test]
        public void RetrieveItsListOfWarehousesFromConfiguration()
        {
            Target.Start();

            GetDependencyAsSubstitute<IServiceConfiguration>()
                .Received().GetConfigSetting(ConfigSettingKeys.WarehouseList);
        }

        [Test]
        public void SubscribeToStockChangeEventsForAllItsWarehouses()
        {
            Target.Start();

            GetDependencyAsSubstitute<IAzureTopicSubscriber>()
                .Received().Subscribe(Arg.Is<SubscriptionCreationArgs>(arg => arg.ContractType == typeof(StockChangeEventV1)));

        }

        [Test]
        public void PersistACopyOfTheStockChangeMessage()
        {
            
        }
    }
}
