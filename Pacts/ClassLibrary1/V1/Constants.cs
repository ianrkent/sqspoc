using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PoC.Contracts.V1
{
    public static class Constants
    {
        public static JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateParseHandling = DateParseHandling.DateTime,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }
}