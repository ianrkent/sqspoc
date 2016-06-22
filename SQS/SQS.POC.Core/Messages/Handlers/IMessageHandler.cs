using System.Threading.Tasks;

namespace SQS.POC.Core
{
    public interface IMessageHandler<in TMessage>
    {
        void Init();

        Task HandleMessage(TMessage message);
    }
}