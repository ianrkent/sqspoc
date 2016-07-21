using Nancy;
using PoC.Sqs.Core.Adapters;

namespace PoC.Sqs.Core.HttpApi
{
    public class ManagementModule : NancyModule
    {
        public ManagementModule(IDependencyHealthCheck dbHealthCheck) : base("/manage")
        {
            Get["/status"] = x => dbHealthCheck.IsHealthy
                    ? new {Status = "Healthy"}
                    : new {Status = "Unhealthy"};
        }
    }
}