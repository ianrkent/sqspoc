namespace SQS.POC.Core.Messages
{
    public class StockChangeEventV1
    {
        public string Sku { get; set; }
        public string WarehouseId { get; set; }
        public int InStockQty { get; set; }
        public int ReservedQty { get; set; }
        public int AllocatedQty { get; set; }
    }
}