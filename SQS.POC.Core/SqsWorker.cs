using System;
using System.Linq;
using SQS.POC.Core.Adapters.Configuration;
using SQS.POC.Core.Adapters.DataStore;
using SQS.POC.Core.Adapters.Messaging;
using SQS.POC.Core.Entities;
using SQS.POC.Core.Messages;

namespace SQS.POC.Core
{
    public class SqsWorker
    {
        private readonly IServiceConfiguration _configuration;
        private readonly IAzureTopicSubscriber _topicSubscriber;
        private readonly IStockQuantityCommand _command;
        private readonly IStockQuantityQuery _stockQuantityQuery;

        public SqsWorker(IServiceConfiguration configuration, IAzureTopicSubscriber topicSubscriber, IStockQuantityCommand command, IStockQuantityQuery stockQuantityQuery)
        {
            _configuration = configuration;
            _topicSubscriber = topicSubscriber;
            _command = command;
            _stockQuantityQuery = stockQuantityQuery;
        }

        public void Start()
        {
            var warehouses = _configuration.GetConfigSetting(ConfigSettingKeys.WarehouseList).Split(',').Select(v => v.Trim());
            var filterClause = string.Join(" OR ", warehouses.Select(warehouse => $"WarehouseId = {warehouse}"));

            Action<StockChangeEventV1> messageHandler = stockChangeEvent =>
            {
                var existing = _stockQuantityQuery.GetSingle(stockChangeEvent.Sku, stockChangeEvent.WarehouseId);
                if (existing == null)
                {
                    _command.Insert(new StockQuantityEntity
                    {
                        WarehouseId = stockChangeEvent.WarehouseId,
                        Sku = stockChangeEvent.Sku
                    });
                }
                else
                {
                    _command.Update(existing);
                }
            };

            _topicSubscriber.Subscribe(new SubscriptionCreationArgs
            {
                ContractType = typeof(StockChangeEventV1),
                MessageHandler = messageHandler,
                Filter = filterClause
            });
        }
    }
}

