using System.Threading.Tasks;
using PoC.Sqs.Core.Entities;

namespace PoC.Sqs.Core.Adapters.DataStore
{
    public interface IStockQuantityQuery    
    {
        Task<StockQuantityEntity> GetSingle(string sku, string warehouseId);
    }
}