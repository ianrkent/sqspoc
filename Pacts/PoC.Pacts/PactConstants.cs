using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PoC.Pacts
{
    public static class PactConstants
    {
        public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTime,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static class PactProviderNames
        {
            public static string SqsPocApi = "Stock Quantity Service API";
        }

        public static class PactConsumerNames
        {
            public static string Product = "Product";
        }

        public static string PactRootLocation = "c:\\pacts";
    }
}