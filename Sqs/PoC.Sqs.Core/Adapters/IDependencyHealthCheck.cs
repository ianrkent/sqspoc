namespace PoC.Sqs.Core.Adapters
{
    public interface IDependencyHealthCheck
    {
        bool IsHealthy { get; }
    }
}
