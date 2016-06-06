using System;
using FluentAssertions.Events;
using NSubstitute;
using NSubstitute.Core.Arguments;
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
    public class TheSqsServiceShould : AutoSubstituteTestBase<SqsWorker>
    {
        private SqsWorker TheSqsWorker => Sut;

        [Test]
        public void RetrieveItsListOfWarehousesFromConfiguration()
        {
            TheSqsWorker.Start();

            TheDependency<IServiceConfiguration>()
                .Received().GetConfigSetting(ConfigSettingKeys.WarehouseList);
        }

        [Test]
        public void SubscribeToStockChangeEventsForAllItsWarehouses()
        {
            TheSqsWorker.Start();

            TheDependency<IAzureTopicSubscriber>()
                .Received()
                .Subscribe(Arg.Is<SubscriptionCreationArgs>(arg => arg.ContractType == typeof (StockChangeEventV1)));

        }

        [Test]
        public void InsertAWarehouseStockEntryWhenAStockChangeEventIsReceivedAndItDoesntExist()
        {
            const string sku = "1234";
            const string warehouseId = "FC01";

            // capture the callback that is being used as a message handler for the Azure subscription
            Action<StockChangeEventV1> subscriptionMessageHandler = null;
            TheDependency<IAzureTopicSubscriber>()
                .Subscribe(
                    Arg.Do<SubscriptionCreationArgs>(
                        creationArgs => subscriptionMessageHandler = creationArgs.MessageHandler));
            TheDependency<IStockQuantityQuery>().GetSingle(sku, warehouseId).Returns(null as StockQuantityEntity);

            // act
            TheSqsWorker.Start();
            subscriptionMessageHandler(new StockChangeEventV1 { Sku = sku, WarehouseId = warehouseId });

            // assert
            TheDependency<IStockQuantityCommand>()
                .Received(1)
                .Insert(Arg.Is<StockQuantityEntity>(arg => arg.WarehouseId == warehouseId && arg.Sku == sku));
        }

        [Test]
        public void UpdateAWarehouseStockEntryWhenAStockChangeEventIsReceivedAndItDoesExist()
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

            Action<StockChangeEventV1> subscriptionMessageHandler = null;
            TheDependency<IAzureTopicSubscriber>()
                .Subscribe(Arg.Do<SubscriptionCreationArgs>(creationArgs => subscriptionMessageHandler = creationArgs.MessageHandler));
            TheDependency<IStockQuantityQuery>().GetSingle(sku, warehouseId).Returns(existing);

            // act
            TheSqsWorker.Start();

            subscriptionMessageHandler(message);

            // assert
            TheDependency<IStockQuantityCommand>()
                .Received(1)
                .Update(Arg.Is<StockQuantityEntity>(arg => arg.WarehouseId == warehouseId && arg.Sku == sku));
        }
    }
}
