using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using PoC.Sqs.Core.Adapters.Configuration;
using PoC.Sqs.Core.Adapters.Messaging;
using PoC.Sqs.Core.Messages;
using PoC.Sqs.Core.Messages.Handlers;

namespace PoC.Sqs.Tests.Inner
{
    [TestFixture]
    public class StockChangeHandlerShould : AutoSubstituteTestBase<StockChangeHandler>
    {
        private StockChangeHandler TheStockChangeEventHandler => Sut;

        [Test]
        public void IdentifyItsWarehousesFromConfiguration()
        {
            TheStockChangeEventHandler.Init();
            TheDependency<IServiceConfiguration>().Received().GetConfigSetting(ConfigSettingKeys.WarehouseList);
        }

        [Test]
        public void CreateASubscriptionForTheStockChangeEventV1AgainstTheConfiguredWarehouses()
        {
            var allSubscriptionCreationArgs = new List<SubscriptionCreationArgs>();
            var warehousesToSupport = new[] {"FC01", "FC04"};

            TheDependency<IServiceConfiguration>().GetConfigSetting(ConfigSettingKeys.WarehouseList).Returns(string.Join(", ", warehousesToSupport));
            TheDependency<IAzureTopicSubscriber>().Subscribe(Arg.Do<SubscriptionCreationArgs>(args => allSubscriptionCreationArgs.Add(args)));

            TheStockChangeEventHandler.Init();

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
