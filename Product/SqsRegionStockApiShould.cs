using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using PactNet;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using PoC.Pacts;
using Product.Tests.Client;
using ServiceStatus = Product.Tests.Client.ServiceStatus;

namespace Product.Tests
{
    [TestFixture]
    public class SqsRegionStockApiShould
    {
        public const int MockSqsApiPort = 1234;

        private IPactBuilder _pactBuilder;
        private IMockProviderService _mockProviderService;
        private SqsRegionStockApiClient _sqsRegionStockApiClient;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            var pactConfig = new PactConfig { PactDir = $"{ PactConstants.PactRootLocation}\\{PactConstants.PactProviderNames.SqsPocApi}\\{PactConstants.PactConsumerNames.Product}", LogDir = $"{ PactConstants.PactRootLocation }\\logs" };
            _pactBuilder = new PactBuilder(pactConfig);

            _pactBuilder
                .ServiceConsumer(PactConstants.PactConsumerNames.Product)
                .HasPactWith(PactConstants.PactProviderNames.SqsPocApi);

            _mockProviderService = _pactBuilder.MockService(
                MockSqsApiPort,
                PactConstants.JsonSettings);
        }

        [SetUp]
        public void TestSetup()
        {
            _mockProviderService.ClearInteractions();
            _sqsRegionStockApiClient = new SqsRegionStockApiClient($"http://{Environment.MachineName}:{MockSqsApiPort}");
        }

        [Ignore("not ready for having this in the pact")]
        [Test]
        public void Return_RegionStock_Status_WHEN_Variant_Is_Found()
        {
            // Arrange
            var variantId = 4567;
            var regionId = "US";
            var stockStatus = "InStock";

            _mockProviderService
                .Given($"Variant {variantId} exists in region {regionId} and has status {stockStatus}")
                .UponReceiving($"A GET request to retrieve the RegionStock status for variant {variantId} to region {regionId}")
                .With(BuildRegionStockStatusRequest(regionId, variantId))
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new {
                        variantId,
                        status = "InStock"
                    }
                });

            // Act
            var regionStock = _sqsRegionStockApiClient.GetRegionStockStatus(regionId, variantId);

            // Assert
            regionStock.Should().NotBeNull();
            regionStock.Value.Should().Be(RegionStockStatus.InStock);
            _mockProviderService.VerifyInteractions();

        }

        [Test]
        public void Return_HealthyStatus_When_There_Are_No_Problems()
        {
            var providerServiceRequest = BuildHealthStatusRequest();

            _mockProviderService
                .Given("The service is up and running and all dependencies are available")
                .UponReceiving("A request for the status")
                .With(providerServiceRequest)
                .WillRespondWith(new ProviderServiceResponse()
                {
                    Status = 200,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new
                    {
                        status = "Healthy"
                    }
                });

            var sqsServiceStatus = _sqsRegionStockApiClient.GetServiceStatus();

            sqsServiceStatus.Should().Be(ServiceStatus.Healthy);
        }

        [Test]
        public void Return_UnHealthyStatus_When_There_Are_Problems()
        {
            var providerServiceRequest = BuildHealthStatusRequest();

            _mockProviderService
                .Given("The service is up and running and some dependencies are not available")
                .UponReceiving("A request for the status")
                .With(providerServiceRequest)
                .WillRespondWith(new ProviderServiceResponse()
                {
                    Status = 200,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new
                    {
                        status = "Unhealthy"
                    }
                });

            var sqsServiceStatus = _sqsRegionStockApiClient.GetServiceStatus();

            sqsServiceStatus.Should().Be(ServiceStatus.Unhealthy);
        }

        private static ProviderServiceRequest BuildRegionStockStatusRequest(string regionId, int variantId)
        {
            return new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path =$"/RegionStock/{regionId}/Status",
                Headers = new Dictionary<string, string>
                {
                    { "Accept", "application/json" }
                },
                Query = $"variantId={ variantId }"
            };
        }

        private static ProviderServiceRequest BuildHealthStatusRequest()
        {
            return new ProviderServiceRequest
            {
                Method = HttpVerb.Get,
                Path = "/manage/status",
                Headers = new Dictionary<string, string>
                {
                    { "Accept", "application/json" }
                },

            };
        }

        [OneTimeTearDown]
        public void TearDown()
        {
           _pactBuilder.Build();     
        }
    }
}
