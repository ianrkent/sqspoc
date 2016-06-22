using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using SQS.POC.Core.Entities;

namespace SQS.POC.Core.Adapters.DataStore.DocumentDb
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
            return new DocumentClient(new Uri("https://sqs-poc.documents.azure.com:443/"), "JSZyonD9aBOPRJxDrOgoUCPwQY4tm3COkv9DdWY3OGOX0u93nbVSJinWaA9OpLtT9HafPf7mOWrfKfhmsjAgDg==");
        }
    }
}