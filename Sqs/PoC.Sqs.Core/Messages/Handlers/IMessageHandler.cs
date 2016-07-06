using System.Threading.Tasks;

namespace PoC.Sqs.Core.Messages.Handlers
{
    public interface IMessageHandler<in TMessage>
    {
        void Init();

        Task HandleMessage(TMessage message);
    }
}