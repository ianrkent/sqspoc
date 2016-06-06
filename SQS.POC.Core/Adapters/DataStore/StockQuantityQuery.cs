using SQS.POC.Core.Entities;

namespace SQS.POC.Core.Adapters.DataStore
{
    public class StockQuantityQuery  : IStockQuantityQuery
    {
        public StockQuantityEntity GetSingle(string sku, string warehouseId)
        {
            throw new System.NotImplementedException();
        }
    }
}