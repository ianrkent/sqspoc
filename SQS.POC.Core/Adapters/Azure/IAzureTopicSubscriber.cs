namespace SQS.POC.Core.Adapters.Azure
{
    public interface IAzureTopicSubscriber
    {
        void Subscribe(SubscriptionCreationArgs subscriptionCreationArgs);
    }
}