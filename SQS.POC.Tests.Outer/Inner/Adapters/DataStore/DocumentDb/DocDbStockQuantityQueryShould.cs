using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;
using SQS.POC.Core.Adapters.DataStore.DocumentDb;
using SQS.POC.Core.Entities;

namespace SQS.POC.Tests.Inner.Adapters.DataStore.DocumentDb
{
    [TestFixture]
    public class DocDbStockQuantityQueryShould 
    {
        private DocumentClient docDbClient;
        private StockQuantityQuery target;

        private const string TestDbName = "TestDB";

        [SetUp]
        public void Setup()
        {
            docDbClient = new DocumentClient(new Uri("https://sqs-poc.documents.azure.com:443/"),
                "JSZyonD9aBOPRJxDrOgoUCPwQY4tm3COkv9DdWY3OGOX0u93nbVSJinWaA9OpLtT9HafPf7mOWrfKfhmsjAgDg==");

            docDbClient.EnsureDatabase(TestDbName).Wait();

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
            await docDbClient.DeleteDocumentCollectionIfExists(TestDbName, "aTestCollection");
            
            var result = await target.GetSingle("abc", "FC01");

            result.Should().BeNull("the document collection doesn't exist");
        }

        [Test]
        public async Task ReturnNullIfTheDocumentIsNotFound()
        {
            await docDbClient.EnsureCollection(TestDbName, "aTestCollection");

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

            await docDbClient.EnsureDocument(TestDbName, "aTestCollection", existing );

            var result = await target.GetSingle(existing.Sku, existing.WarehouseId);

            result.ShouldBeEquivalentTo(existing);
        }
    }

    
}
