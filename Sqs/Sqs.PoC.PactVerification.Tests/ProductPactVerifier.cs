using Microsoft.Owin.Testing;
using NUnit.Framework;
using PactNet;
using PoC.Sqs.Core.Infrastructure.Owin;

namespace PoC.Sqs.PactVerification.Tests
{
    [TestFixture]
    public class ProductPactVerifier
    {
        [Test]
        public void VerifyAllPacts()
        {
            //Arrange
            IPactVerifier pactVerifier = new PactVerifier(() => { }, () => { });

            // pactVerifier.ProviderState("There is a something with id 'tester'", stateSetup, stateTeardown);

            //Act / Assert
            using (var testServer = TestServer.Create<Startup>()) //NOTE: This is using the Microsoft.Owin.Testing to test host an owin app :)
            {
                pactVerifier
                    .ServiceProvider("Something API", testServer.HttpClient)
                    .HonoursPactWith("Consumer")
                    .PactUri("../../../Consumer.Tests/pacts/consumer-something_api.json")
                    //or
                    .PactUri("http://pact-broker/pacts/provider/Something%20Api/consumer/Consumer/version/latest") //You can specify a http or https uri
                                                                                                                   //or
                    .PactUri("http://pact-broker/pacts/provider/Something%20Api/consumer/Consumer/version/latest", new PactUriOptions("someuser", "somepassword")) //You can also specify http/https basic auth details
                    .Verify(); //NOTE: Optionally you can control what interactions are verified by specifying a providerDescription and/or providerState
            }
        }
    }
}
