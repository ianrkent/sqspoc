using Nancy;
using PoC.Sqs.Core.Adapters;

namespace PoC.Sqs.Core.HttpApi
{
    public class ManagementModule : NancyModule
    {
        public ManagementModule(IAdapterHealthCheck dbAdapter) : base("/manage")
        {
            Get["/status"] = x => 
                new
                {
                    Status = dbAdapter.IsAvailable() ? "Healthy" : "Unhealthy"
                };
        }
    }
}