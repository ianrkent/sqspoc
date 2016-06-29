using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using SQS.POC.Core.Entities;

namespace SQS.POC.Tests.Inner.Adapters.DataStore.DocumentDb
{
    public static class DocDbExtensions
    {
        public static async Task<string> EnsureDatabase(this DocumentClient docDbClient, string databaseName)
        {
            var database = docDbClient.CreateDatabaseQuery().Where(db => db.Id == databaseName).AsEnumerable().FirstOrDefault() ??
                           await docDbClient.CreateDatabaseAsync(new Database { Id = databaseName });

            return database.SelfLink;
        }

        public static async Task<string> EnsureCollection(this DocumentClient client, string databaseLink, string collectionName)
        {
            var collectionLink = await client.GetCollectionLink(databaseLink, collectionName);

            if (!string.IsNullOrWhiteSpace(collectionLink))
            {
                return collectionLink;
            }

            DocumentCollection collection = await client.CreateDocumentCollectionAsync(
                databaseLink,
                new DocumentCollection {Id = collectionName});

            return collection.SelfLink;
        }

        public static async Task DeleteDocumentCollectionIfExists(this DocumentClient docDbClient, string databaseLink, string collectionName)
        {
            var collectionLink = await docDbClient.GetCollectionLink(databaseLink, collectionName);

            if (!string.IsNullOrWhiteSpace(collectionLink))
            {
                await docDbClient.DeleteDocumentCollectionAsync(collectionLink);
            }
        }

        public static async Task EnsureDocument<TDocument>(this DocumentClient client, string collectionLink, TDocument document) where TDocument : IPersistableEntity
        {
            await client.CreateDocumentAsync(collectionLink, document);
        }

        public static async Task DeleteDocuments<TDocument>(this DocumentClient client, string collectionLink, Expression<Func<TDocument, bool>> filterPredicate) where TDocument : IPersistableEntity
        {
            //var results = client.CreateDocumentQuery<TDocument>(collectionLink, new FeedOptions { MaxItemCount = 10 }).AsDocumentQuery();
            //while (results.HasMoreResults)
            //{
            //    var feedResponse = await results.ExecuteNextAsync<TDocument>();
            //    foreach (TDocument item in feedResponse)
            //    {
                    
            //        // var doc = (Document) item;
            //        // await client.DeleteDocumentAsync(((Document)item).SelfLink);
            //    }
            //}


            var results2 = client.CreateDocumentQuery<TDocument>(collectionLink, new FeedOptions { MaxItemCount = 10 })
                .Where(filterPredicate)
                .AsDocumentQuery();

            while (results2.HasMoreResults)
            {
                var feedResponse = await results2.ExecuteNextAsync<Document>();
                foreach (var item in feedResponse)
                {
                    await client.DeleteDocumentAsync(item.SelfLink);
                }
            }
        }

        public static async Task<string> GetCollectionLink(this DocumentClient client, string databaseLink, string collectionName)
        {
            var collections = await client.ReadDocumentCollectionFeedAsync(databaseLink);
            return collections
                .FirstOrDefault(collection => collection.Id == collectionName)
                ?.SelfLink;
        }
    }
}