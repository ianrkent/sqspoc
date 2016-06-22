using System.Threading.Tasks;
using SQS.POC.Core.Entities;

namespace SQS.POC.Core.Adapters.DataStore.DocumentDb
{
    public class StockQuantityCommand  : IStockQuantityCommand
    {
        public void Insert(StockQuantityEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public async Task Update(StockQuantityEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}