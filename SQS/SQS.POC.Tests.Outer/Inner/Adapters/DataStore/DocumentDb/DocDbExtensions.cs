using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace SQS.POC.Tests.Inner.Adapters.DataStore.DocumentDb
{
    public static class DocDbExtensions
    {
        public static async Task DeleteDocumentCollectionIfExists(this DocumentClient docDbClient, string databaseName, string collectionName)
        {
            if (await docDbClient.CollectionExists(databaseName, collectionName))
            {
                await docDbClient.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
            }
        }

        public static async Task EnsureDatabase(this DocumentClient docDbClient, string databaseName)
        {
            var database = docDbClient.CreateDatabaseQuery().Where(db => db.Id == databaseName).AsEnumerable().FirstOrDefault();
            if (database == null)
            {
                await docDbClient.CreateDatabaseAsync(new Database { Id = databaseName });
            }
        }

        public static async Task EnsureCollection(this DocumentClient client, string databaseName, string collectionName)
        {
            if (!await client.CollectionExists(databaseName, collectionName))
            {
                await client.CreateDocumentCollectionAsync(
                    UriFactory.CreateDatabaseUri(databaseName),
                    new DocumentCollection { Id = collectionName });
            }
        }

        public static async Task EnsureDocument<TDocument>(this DocumentClient client, string databaseName, string collectionName, TDocument document)
        {
            await client.EnsureCollection(databaseName, collectionName);
            await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), document);
        }

        public static async Task<bool> CollectionExists(this DocumentClient client, string databaseName, string collectionName)
        {
            var collections = await client.ReadDocumentCollectionFeedAsync(UriFactory.CreateDatabaseUri(databaseName));
            return collections.Any(collection => collection.Id == collectionName);
        }
    }
}