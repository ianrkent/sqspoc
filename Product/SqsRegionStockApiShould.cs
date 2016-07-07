using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using PactNet;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Product.Tests.Client;

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
            var pactConfig = new PactConfig { PactDir = $"{ Constants.PactRootLocation}\\{Constants.PactProviderNames.SqsPocApi}\\{Constants.PactConsumerNames.Product}", LogDir = $"{ Constants.PactRootLocation }\\logs" };
            _pactBuilder = new PactBuilder(pactConfig);

            _pactBuilder
                .ServiceConsumer(Constants.PactConsumerNames.Product)
                .HasPactWith(Constants.PactProviderNames.SqsPocApi);

            _mockProviderService = _pactBuilder.MockService(
                MockSqsApiPort, 
                Constants.JsonSettings);
        }

        [SetUp]
        public void TestSetup()
        {
            _mockProviderService.ClearInteractions();
            _sqsRegionStockApiClient = new SqsRegionStockApiClient($"http://{Environment.MachineName}:{MockSqsApiPort}");
        }

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
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path =$"/RegionStock/{regionId}/Status",
                    Headers = new Dictionary<string, string>
                    {
                        { "Accept", "application/json" }
                    },
                    Query = $"variantId={ variantId }"
                })
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

        [OneTimeTearDown]
        public void TearDown()
        {
           _pactBuilder.Build();     
        }
    }
}
