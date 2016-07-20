using System;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using PoC.Sqs.Core;
using PoC.Sqs.Core.Adapters.Configuration;
using PoC.Sqs.Core.Adapters.DataStore;
using PoC.Sqs.Core.Adapters.Messaging;
using PoC.Sqs.Core.Entities;
using PoC.Sqs.Core.Messaging;

namespace PoC.Sqs.Tests.Outer
{
    [TestFixture]
    public class TheSqsWorkerServiceShould : OuterTestBase
    {
        private SqsMessagingWorker _theSqsMessagingWorker;

        [SetUp]
        public void Setup()
        {
            _theSqsMessagingWorker = IoCConfig.ResolveService<SqsMessagingWorker>();
        }

        [Test]
        public void RetrieveItsListOfWarehousesFromConfiguration()
        {
            _theSqsMessagingWorker.Start();

            AdapterSubstitute<IServiceConfiguration>()
                .Received().GetConfigSetting(ConfigSettingKeys.WarehouseList);
        }

        [Test]
        public void SubscribeToStockChangeEventsForAllItsWarehouses()
        {
            _theSqsMessagingWorker.Start();

            AdapterSubstitute<IAzureTopicSubscriber>()
                .Received()
                .Subscribe(Arg.Is<SubscriptionCreationArgs>(arg => arg.ContractType == typeof (StockChangeEventV1)));
        }

        [Test]
        public async Task InsertAWarehouseStockEntryWhenAStockChangeEventIsReceivedAndItDoesntExist()
        {
            // arrange
            const string sku = "1234";
            const string warehouseId = "FC01";

            AdapterSubstitute<IStockQuantityQuery>().GetSingle(sku, warehouseId).Returns(null as StockQuantityEntity);

            // act

            // capture the callback that is being used as a message handler for the Azure subscription
            Func<StockChangeEventV1, Task> subscriptionMessageHandler = null;
            AdapterSubstitute<IAzureTopicSubscriber>()
                .Subscribe(
                    Arg.Do<SubscriptionCreationArgs>(
                        creationArgs => subscriptionMessageHandler = creationArgs.MessageHandler));

            _theSqsMessagingWorker.Start();
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
            _theSqsMessagingWorker.Start();
            await subscriptionMessageHandler(message);

            // assert
            await AdapterSubstitute<IStockQuantityCommand>()
                .Received(1)
                .Update(Arg.Is<StockQuantityEntity>(arg => arg.WarehouseId == warehouseId && arg.Sku == sku));
        }
    }
}
