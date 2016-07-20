using Microsoft.Owin.Testing;
using NUnit.Framework;
using PactNet;
using PactNet.Models;
using PoC.Contracts.V1;
using PoC.Sqs.Core.Infrastructure.Owin;

namespace PoC.Sqs.PactVerification.Tests
{
    [TestFixture]
    public class ProductPactVerifier
    {
        [Test]
        public void VerifyAllHealthCheckPacts()
        {
            // Arrange
            IPactVerifier pactVerifier = new PactVerifier(() => { }, () => { });

            var pactDetails = new PactDetails()
            {
                Provider = new Pacticipant { Name = PactConstants.PactProviderNames.SqsPocApi },
                Consumer = new Pacticipant { Name = PactConstants.PactConsumerNames.Product }
            };
            var pactLocation = $"{ PactConstants.PactRootLocation }\\{ PactConstants.PactProviderNames.SqsPocApi }\\{ PactConstants.PactConsumerNames.Product }\\{ pactDetails.GeneratePactFileName() }";

            pactVerifier
                .ProviderState("The service is up and running and all dependencies are available")
                .ProviderState("The service is up and running and some dependencies are not available", SetupDependenciesNotAvailable);

            // Act & Assert
            using (var testServer = TestServer.Create<Startup>()) //NOTE: This is using the Microsoft.Owin.Testing to test host an owin app :)
            {
                pactVerifier = pactVerifier
                    .ServiceProvider(pactDetails.Provider.Name, testServer.HttpClient)
                    .HonoursPactWith(pactDetails.Consumer.Name)
                    .PactUri(pactLocation);

                pactVerifier.Verify("A request for the status");
            }
        }

        private void SetupDependenciesNotAvailable()
        {
            throw new System.NotImplementedException();
        }
    }
}
