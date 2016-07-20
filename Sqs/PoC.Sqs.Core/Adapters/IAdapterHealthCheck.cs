namespace PoC.Sqs.Core.Adapters
{
    public interface IAdapterHealthCheck
    {
        bool IsAvailable();
    }
}