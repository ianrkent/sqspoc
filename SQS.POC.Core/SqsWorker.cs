using SQS.POC.Core.Adapters.Azure;
using SQS.POC.Core.Adapters.Configuration;
using SQS.POC.Core.Messages;

namespace SQS.POC.Core
{
    public class SqsWorker
    {
        private readonly IServiceConfiguration _configuration;
        private readonly IAzureTopicSubscriber _topicSubscriber;

        public SqsWorker(IServiceConfiguration configuration, IAzureTopicSubscriber topicSubscriber)
        {
            _configuration = configuration;
            _topicSubscriber = topicSubscriber;
        }

        public void Start()
        {
            _configuration.GetConfigSetting(ConfigSettingKeys.WarehouseList);
            _topicSubscriber.Subscribe(new SubscriptionCreationArgs
            {
                ContractType = typeof(StockChangeEventV1)
            });
        }
    }
}