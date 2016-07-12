using Nancy;

namespace PoC.Sqs.Core.HttpApi
{
    public class ManagementModule : NancyModule
    {
        public ManagementModule() : base("/manage")
        {
            Get["/status"] = x => "Healthy";
        }
    }
}