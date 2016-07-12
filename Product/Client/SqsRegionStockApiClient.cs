using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using PoC.Contracts;
using PoC.Pacts;

namespace Product.Tests.Client
{
    public class SqsRegionStockApiClient    
    {
        private readonly HttpClient _httpClient;

        public SqsRegionStockApiClient(string baseUri)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUri) };
        }

        public RegionStockStatus? GetRegionStockStatus(string regionId, int variantId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"RegionStock/{regionId}/Status?variantId={variantId}");
            request.Headers.Add("Accept", "application/json");

            var response = _httpClient.SendAsync(request);
            var result = response.Result;

            if (result.StatusCode != HttpStatusCode.OK) return null;

            var responseContent = result.Content.ReadAsStringAsync().Result;
            var resource = JsonConvert.DeserializeObject<RegionStockStatusResource>(responseContent, PactConstants.JsonSettings);
            RegionStockStatus parsedRegionStockStatus;
            if (!Enum.TryParse(resource.Status, out parsedRegionStockStatus)) return null;
            
            return parsedRegionStockStatus;
        }

        public ServiceStatus GetServiceStatus()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"manage/status");
            request.Headers.Add("Accept", "application/json");

            var response = _httpClient.SendAsync(request);
            var result = response.Result;

            if (result.StatusCode != HttpStatusCode.OK) throw new Exception($"Response code of { result.StatusCode } returned which is not part of the contract");

            var responseContent = result.Content.ReadAsStringAsync().Result;
            var resource = JsonConvert.DeserializeObject<ServiceStatusResponse>(responseContent, PactConstants.JsonSettings);

            ServiceStatus parsedStatus;
            if (!Enum.TryParse(resource.Status, out parsedStatus)) throw new Exception($"Unable to parse response status of { resource.Status }"); ;

            return parsedStatus;
        }
    }
}