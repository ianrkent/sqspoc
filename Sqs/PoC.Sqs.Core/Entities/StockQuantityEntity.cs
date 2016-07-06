using System;
using Newtonsoft.Json;

namespace PoC.Sqs.Core.Entities
{
    public class StockQuantityEntity : IPersistableEntity
    {
        public string WarehouseId { get; set; }
        public string Sku { get; set; }
        public int AvailableQty { get; set; }
        public int ReservedQty { get; set; }
        public Guid Id { get; set; }
    }

    public interface IPersistableEntity
    {
        [JsonProperty(PropertyName = "id")]
        Guid Id { get; set; }
    }
}