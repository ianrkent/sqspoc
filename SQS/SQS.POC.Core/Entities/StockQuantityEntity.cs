using System;
using Newtonsoft.Json;

namespace SQS.POC.Core.Entities
{
    public class StockQuantityEntity
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } 
        public string WarehouseId { get; set; }
        public string Sku { get; set; }
        public int AvailableQty { get; set; }
        public int ReservedQty { get; set; }
    }
}