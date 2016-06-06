using SQS.POC.Core.Entities;

namespace SQS.POC.Core.Adapters.DataStore
{
    public interface IStockQuantityCommand  
    {
        void Insert(StockQuantityEntity entity);
        void Update(StockQuantityEntity entity);
    }
}