namespace SQS.POC.Core.Adapters.Messaging
{
    public interface IAzureTopicSubscriber
    {
        void Subscribe(SubscriptionCreationArgs subscriptionCreationArgs);
    }
}