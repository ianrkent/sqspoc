using System.Linq;
using SQS.POC.Core.Adapters.Configuration;
using SQS.POC.Core.Adapters.DataStore;
using SQS.POC.Core.Adapters.Messaging;
using SQS.POC.Core.Entities;
using SQS.POC.Core.Messages;

namespace SQS.POC.Core
{
    public class StockChangeHandler : IMessageHandler<StockChangeEventV1>
    {
        private readonly IServiceConfiguration _configuration;
        private readonly IAzureTopicSubscriber _topicSubscriber;
        private readonly IStockQuantityCommand _command;
        private readonly IStockQuantityQuery _stockQuantityQuery;

        public StockChangeHandler(IServiceConfiguration configuration, IAzureTopicSubscriber topicSubscriber, IStockQuantityCommand command, IStockQuantityQuery stockQuantityQuery)
        {
            _configuration = configuration;
            _topicSubscriber = topicSubscriber;
            _command = command;
            _stockQuantityQuery = stockQuantityQuery;
        }

        public void Init()
        {
            var warehouses = _configuration.GetConfigSetting(ConfigSettingKeys.WarehouseList).Split(',').Select(v => v.Trim());
            var filterClause = string.Join(" OR ", warehouses.Select(warehouse => $"WarehouseId = {warehouse}"));

            _topicSubscriber.Subscribe(new SubscriptionCreationArgs
            {
                ContractType = typeof(StockChangeEventV1),
                MessageHandler = HandleMessage,
                Filter = filterClause
            });
        }

        public void HandleMessage(StockChangeEventV1 message)
        {
            var existing = _stockQuantityQuery.GetSingle(message.Sku, message.WarehouseId);
            if (existing == null)
            {
                _command.Insert(new StockQuantityEntity
                {
                    WarehouseId = message.WarehouseId,
                    Sku = message.Sku
                });
            }
            else
            {
                _command.Update(existing);
            }
        }
    }
}