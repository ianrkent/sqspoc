using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using Nancy.Hosting.Self;

namespace StockQuantity.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("StockQuantity.Worker is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("StockQuantity.Worker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("StockQuantity.Worker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("StockQuantity.Worker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {

            const string endpointName = "Management";
            var roleInstance = RoleEnvironment.CurrentRoleInstance;
            var publicEndpoint = roleInstance.InstanceEndpoints[endpointName].IPEndpoint;
            var uri = new Uri("http://" + publicEndpoint);

            using (var host = new NancyHost(uri))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Trace.TraceInformation("Working");
                    await Task.Delay(1000);
                }
            }
        }
    }
}
    