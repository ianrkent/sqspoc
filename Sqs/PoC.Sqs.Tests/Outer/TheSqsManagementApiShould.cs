using FluentAssertions;
using Nancy.Testing;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using PoC.Sqs.Core;
using PoC.Sqs.Core.Adapters;
using PoC.Sqs.Core.HttpApi;
using PoC.Sqs.Core.Infrastructure.Owin;

namespace PoC.Sqs.Tests.Outer
{
    [TestFixture]
    public class TheSqsManagementApiShould : OuterTestBase
    {
        [Test]
        public void ReturnHealthyStatusWhenDocDbIsAvailable()
        {
            var browser = new Browser(new CustomAutofacNancyBootstrapper());
            AdapterSubstitute<IAdapterHealthCheck>().IsAvailable().Returns(true);
             
            var response = browser.Get("manage/status", browserContext =>  browserContext.Header("Accept", "application/json"));

            var statusResponse = JsonConvert.DeserializeAnonymousType(response.Body.AsString(), new { Status = ""});
            statusResponse.Status.Should().Be("Healthy");
        }

        [Test]
        public void ReturnUnHealthyStatusWhenDocDbIsNotAvailable()
        {
            AdapterSubstitute<IAdapterHealthCheck>().IsAvailable().Returns(false);
            var browser = new Browser(new CustomAutofacNancyBootstrapper());

            var response = browser.Get("manage/status", browserContext => browserContext.Header("Accept", "application/json"));

            var statusResponse = JsonConvert.DeserializeAnonymousType(response.Body.AsString(), new { Status = "" });
            statusResponse.Status.Should().Be("Unhealthy");
        }

    }
}