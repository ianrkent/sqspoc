using Nancy;

namespace Poc.Sqs.Worker.NancyModules
{
    public class ManagementModule : NancyModule
    {
        public ManagementModule() : base("/manage")
        {
            Get["/status"] = x => "Status is 'Up and running'";
        }
    }
}