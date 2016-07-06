using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using PoC.Sqs.Core.Entities;

namespace PoC.Sqs.Core.Adapters.DataStore.DocumentDb
{
    public class StockQuantityQuery  : IStockQuantityQuery
    {
        private const string DbName = "TestDB";
        private const string CollectionName = "aTestCollection";

        public async Task<StockQuantityEntity> GetSingle(string sku, string warehouseId)
        {
            DocumentClient client;
            using (client = GetDocumentClient())
            {
                var collection = client
                    .CreateDocumentCollectionQuery(UriFactory.CreateDatabaseUri(DbName))
                    .Where(c => c.Id == CollectionName)
                    .ToArray()
                    .SingleOrDefault();

                if (collection == null)
                {
                    return null;
                }

                return client
                    .CreateDocumentQuery<StockQuantityEntity>(UriFactory.CreateDocumentCollectionUri(DbName, CollectionName), new FeedOptions())
                    .Where(entity => entity.WarehouseId == warehouseId && entity.Sku == sku)
                    .ToArray()
                    .FirstOrDefault();
            }
        }

        private static DocumentClient GetDocumentClient()
        {
            return new DocumentClient(new Uri("https://sqs-pact-poc.documents.azure.com:443/"), "ss7aB43UfyFCBWccAJUqo972Am7qOMFJaD2RNgq1LWHTOZqWHtnmQ1rhP4qvDODq0ccybhtFAtFJRnrdUhg9AA==");
        }
    }
}