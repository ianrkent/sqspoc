using System.Threading.Tasks;

namespace PoC.Sqs.Core.Messaging.Handlers
{
    public interface IMessageHandler<in TMessage>
    {
        void Init();

        Task HandleMessage(TMessage message);
    }
}