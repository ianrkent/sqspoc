using SQS.POC.Core.Entities;

namespace SQS.POC.Core.Adapters.DataStore
{
    public interface IStockQuantityQuery    
    {
        StockQuantityEntity GetSingle(string sku, string warehouseId);
    }
}