using System.Linq;
using System.Threading.Tasks;
using PoC.Sqs.Core.Adapters.Configuration;
using PoC.Sqs.Core.Adapters.DataStore;
using PoC.Sqs.Core.Adapters.Messaging;
using PoC.Sqs.Core.Entities;

namespace PoC.Sqs.Core.Messaging.Handlers
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

        public async Task HandleMessage(StockChangeEventV1 message)
        {
            var existing = await _stockQuantityQuery.GetSingle(message.Sku, message.WarehouseId);
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
                await _command.Update(existing);
            }
        }
    }
}