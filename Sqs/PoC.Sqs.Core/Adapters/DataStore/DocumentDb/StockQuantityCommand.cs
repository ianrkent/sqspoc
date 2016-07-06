using System.Threading.Tasks;
using PoC.Sqs.Core.Entities;

namespace PoC.Sqs.Core.Adapters.DataStore.DocumentDb
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