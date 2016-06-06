using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using SQS.POC.Core;
using SQS.POC.Core.Adapters.Configuration;
using SQS.POC.Core.Adapters.Messaging;
using SQS.POC.Core.Messages;

namespace SQS.POC.Tests.Inner
{
    [TestFixture]
    public class SqsWorkerShould : AutoSubstituteTestBase<SqsWorker>
    {
        private SqsWorker TheSqsWorker => Sut;

        [Test]
        public void IdentifyItsWarehousesFromConfiguration()
        {
            TheSqsWorker.Start();
            TheDependency<IServiceConfiguration>().Received().GetConfigSetting(ConfigSettingKeys.WarehouseList);
        }

        [Test]
        public void CreateASubscriptionForTheStockChangeEventV1AgainstItsWarehouses()
        {
            var allSubscriptionCreationArgs = new List<SubscriptionCreationArgs>();
            var warehousesToSupport = new[] {"FC01", "FC04"};

            TheDependency<IServiceConfiguration>().GetConfigSetting(ConfigSettingKeys.WarehouseList).Returns(string.Join(", ", warehousesToSupport));
            TheDependency<IAzureTopicSubscriber>().Subscribe(Arg.Do<SubscriptionCreationArgs>(args => allSubscriptionCreationArgs.Add(args)));

            TheSqsWorker.Start();

            var expectedFilter = string.Join(" OR ", warehousesToSupport.Select(warehouse => $"WarehouseId = {warehouse}"));
            allSubscriptionCreationArgs.Should()
                .NotBeEmpty().And
                .Contain(
                    x =>
                        x.ContractType == typeof (StockChangeEventV1) &&
                        x.Filter.Contains(expectedFilter));
        }

    }
}
