namespace SQS.POC.Core
{
    public interface IMessageHandler<in TMessage>
    {
        void Init();

        void HandleMessage(TMessage message);
    }
}