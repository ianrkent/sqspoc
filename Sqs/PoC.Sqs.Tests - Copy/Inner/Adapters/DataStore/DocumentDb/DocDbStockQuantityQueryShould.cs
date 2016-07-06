using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using PoC.Sqs.Core.Adapters.DataStore.DocumentDb;
using PoC.Sqs.Core.Entities;

namespace PoC.Sqs.Tests.Inner.Adapters.DataStore.DocumentDb
{
    [TestFixture]
    public class DocDbStockQuantityQueryShould 
    {
        private const string StockQuantityCollectionName = "aTestCollection";
        private DocumentClient docDbClient;
        private StockQuantityQuery target;

        private string _databaseUri;

        [SetUp]
        public void Setup()
        {
            docDbClient = new DocumentClient(new Uri("https://sqs-pact-poc.documents.azure.com:443/"),
                "ss7aB43UfyFCBWccAJUqo972Am7qOMFJaD2RNgq1LWHTOZqWHtnmQ1rhP4qvDODq0ccybhtFAtFJRnrdUhg9AA==");

            _databaseUri = docDbClient.EnsureDatabase("TestDB").Result;

            target = new StockQuantityQuery();
        }

        [TearDown]
        public void TearDown()
        {
            docDbClient?.Dispose();
        }

        [Test]
        public async Task ReturnNullIfTheDocumentCollectionDoesntExist()
        {
            await docDbClient.DeleteDocumentCollectionIfExists(_databaseUri, StockQuantityCollectionName);
            
            var result = await target.GetSingle("abc", "FC01");

            result.Should().BeNull("the document collection doesn't exist");
        }

        [Test]
        public async Task ReturnNullIfTheDocumentIsNotFound()
        {
            await docDbClient.EnsureCollection(_databaseUri, StockQuantityCollectionName);

            var result = await target.GetSingle("abc", "FC01");

            result.Should().BeNull("the document collection doesn't exist");
        }

        [Test]
        public async Task ReturnTheDocumentIfItExists()
        {
            var existing = new StockQuantityEntity()
            {
                Id = Guid.NewGuid(),
                WarehouseId = "FC01",
                Sku = "ABCDEF",
                AvailableQty = 6,
                ReservedQty = 3
            };

            var collectionLink = await docDbClient.EnsureCollection(_databaseUri, StockQuantityCollectionName);
            await docDbClient.DeleteDocuments<StockQuantityEntity>(collectionLink, sqe => sqe.Sku == existing.Sku && sqe.WarehouseId == existing.WarehouseId);
            await docDbClient.EnsureDocument(collectionLink, existing );

            var result = await target.GetSingle(existing.Sku, existing.WarehouseId);

            result.ShouldBeEquivalentTo(existing);

        }
    }

    
}
