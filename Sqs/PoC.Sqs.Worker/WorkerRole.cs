using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Microsoft.WindowsAzure.ServiceRuntime;
using PoC.Sqs.Core.Infrastructure.Owin;

namespace Poc.Sqs.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private IDisposable _app;

        public override void Run()
        {
            Trace.TraceInformation("Poc.Sqs.Worker is running");

            try
            {
                RunAsync(_cancellationTokenSource.Token).Wait();
            }
            finally
            {
                _runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            var result = base.OnStart();

            StartOwinWebApp();

            Trace.TraceInformation("Poc.Sqs.Worker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("Poc.Sqs.Worker is stopping");

            _app.Dispose();

            _cancellationTokenSource.Cancel();
            _runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("Poc.Sqs.Worker has stopped");
        }

        private void StartOwinWebApp()
        {
            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["Http"];
            var baseUri = $"{endpoint.Protocol}://{endpoint.IPEndpoint}";

            Trace.TraceInformation($"Starting OWIN at {baseUri}", "Information");
            _app = WebApp.Start<Startup>(new StartOptions(url: baseUri));
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}