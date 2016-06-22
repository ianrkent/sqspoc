using System.Threading.Tasks;
using SQS.POC.Core.Entities;

namespace SQS.POC.Core.Adapters.DataStore
{
    public interface IStockQuantityQuery    
    {
        Task<StockQuantityEntity> GetSingle(string sku, string warehouseId);
    }
}