using Nancy;

namespace StockQuantity.Worker
{
    public class ManagementModule : NancyModule
    {
        public ManagementModule()
        {
            Get["/status"] = p => Response.AsText($"Sqs is up and running!");
        }
    }   
}