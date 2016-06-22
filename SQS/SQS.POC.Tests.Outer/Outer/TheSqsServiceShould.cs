using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using SQS.POC.Core;
using SQS.POC.Core.Adapters.Configuration;
using SQS.POC.Core.Adapters.DataStore;
using SQS.POC.Core.Adapters.Messaging;
using SQS.POC.Core.Entities;
using SQS.POC.Core.Messages;

namespace SQS.POC.Tests.Outer
{
    [TestFixture]
    public class TheSqsServiceShould : OuterTestBase
    {
        private SqsWorker TheSqsWorker;

        [SetUp]
        public void Setup()
        {
            TheSqsWorker = IoCConfig.ResolveService<SqsWorker>();
        }

        [Test]
        public void RetrieveItsListOfWarehousesFromConfiguration()
        {
            TheSqsWorker.Start();

            AdapterSubstitute<IServiceConfiguration>()
                .Received().GetConfigSetting(ConfigSettingKeys.WarehouseList);
        }

        [Test]
        public void SubscribeToStockChangeEventsForAllItsWarehouses()
        {
            TheSqsWorker.Start();

            AdapterSubstitute<IAzureTopicSubscriber>()
                .Received()
                .Subscribe(Arg.Is<SubscriptionCreationArgs>(arg => arg.ContractType == typeof (StockChangeEventV1)));
        }

        [Test]
        public async Task InsertAWarehouseStockEntryWhenAStockChangeEventIsReceivedAndItDoesntExist()
        {
            const string sku = "1234";
            const string warehouseId = "FC01";

            // capture the callback that is being used as a message handler for the Azure subscription
            Func<StockChangeEventV1, Task> subscriptionMessageHandler = null;
            AdapterSubstitute<IAzureTopicSubscriber>()
                .Subscribe(
                    Arg.Do<SubscriptionCreationArgs>(
                        creationArgs => subscriptionMessageHandler = creationArgs.MessageHandler));
            AdapterSubstitute<IStockQuantityQuery>().GetSingle(sku, warehouseId).Returns(null as StockQuantityEntity);

            // act
            TheSqsWorker.Start();
            await subscriptionMessageHandler(new StockChangeEventV1 { Sku = sku, WarehouseId = warehouseId });

            // assert
            AdapterSubstitute<IStockQuantityCommand>()
                .Received(1)
                .Insert(Arg.Is<StockQuantityEntity>(arg => arg.WarehouseId == warehouseId && arg.Sku == sku));
        }

        [Test]
        public async Task UpdateAWarehouseStockEntryWhenAStockChangeEventIsReceivedAndItDoesExist()
        {
            const string sku = "1234";
            const string warehouseId = "FC01";
            var existing = new StockQuantityEntity
            {
                Sku = sku,
                WarehouseId = warehouseId,
                AvailableQty = 2,
                ReservedQty = 1
            };
            var message = new StockChangeEventV1
            {
                Sku = sku,
                WarehouseId = warehouseId,
                InStockQty = 2,
                AllocatedQty = 1,
                ReservedQty = 1
            };

            Func<StockChangeEventV1, Task> subscriptionMessageHandler = null;
            AdapterSubstitute<IAzureTopicSubscriber>()
                .Subscribe(Arg.Do<SubscriptionCreationArgs>(creationArgs => subscriptionMessageHandler = creationArgs.MessageHandler));
            AdapterSubstitute<IStockQuantityQuery>().GetSingle(sku, warehouseId).Returns(existing);

            // act
            TheSqsWorker.Start();
            await subscriptionMessageHandler(message);

            // assert
            await AdapterSubstitute<IStockQuantityCommand>()
                .Received(1)
                .Update(Arg.Is<StockQuantityEntity>(arg => arg.WarehouseId == warehouseId && arg.Sku == sku));
        }
    }
}
