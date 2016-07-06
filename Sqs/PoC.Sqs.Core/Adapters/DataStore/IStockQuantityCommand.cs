using System.Threading.Tasks;
using PoC.Sqs.Core.Entities;

namespace PoC.Sqs.Core.Adapters.DataStore
{
    public interface IStockQuantityCommand  
    {
        void Insert(StockQuantityEntity entity);
        Task Update(StockQuantityEntity entity);
    }
}