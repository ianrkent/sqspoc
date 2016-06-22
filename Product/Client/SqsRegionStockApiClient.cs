using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace Product.Tests
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
            var resource = JsonConvert.DeserializeObject<RegionStockStatusResource>(responseContent, Constants.JsonSettings);
            RegionStockStatus parsedRegionStockStatus;
            if (!Enum.TryParse(resource.Status, out parsedRegionStockStatus)) return null;
            
            return parsedRegionStockStatus;
        }

        public class RegionStockStatusResource  
        {
            public int VariantId { get; set; }
            public string Status { get; set; }
        }
    }
}