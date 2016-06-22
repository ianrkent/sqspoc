using System.Threading.Tasks;
using SQS.POC.Core.Entities;

namespace SQS.POC.Core.Adapters.DataStore
{
    public interface IStockQuantityCommand  
    {
        void Insert(StockQuantityEntity entity);
        Task Update(StockQuantityEntity entity);
    }
}