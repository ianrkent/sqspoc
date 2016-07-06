namespace PoC.Sqs.Core.Adapters.Messaging
{
    public interface IAzureTopicSubscriber
    {
        void Subscribe(SubscriptionCreationArgs subscriptionCreationArgs);
    }
}