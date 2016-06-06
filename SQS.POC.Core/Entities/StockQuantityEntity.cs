namespace SQS.POC.Core.Entities
{
    public class StockQuantityEntity
    {
        public string WarehouseId { get; set; }
        public string Sku { get; set; }
        public int AvailableQty { get; set; }
        public int ReservedQty { get; set; }
    }
}