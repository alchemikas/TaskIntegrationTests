using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Product.Api.Client;
using Product.Api.Contract;
using Product.Api.Contract.Responses;
using Product.Api.IntegrationTest.Models;

namespace Product.Api.IntegrationTest
{
    [TestFixture]
    public class ExportProdutTests
    {
        private readonly IProductApiClient _productApiClient;

        public ExportProdutTests()
        {
            _productApiClient = new ProductApiClient();
        }

        private int _productIdToDelete = default(int);

        [SetUp]
        public void Setup()
        {
        }


        [TearDown]
        public async Task TearDown()
        {
            if (_productIdToDelete != default(int))
            {
                await _productApiClient.Delete(_productIdToDelete);
            }
        }


        [Test]
        public async Task WhenExportingProducts_ThenFileIsReturned()
        {
            CreateProduct upcreateModel = ProductModels.GetCreateModel();
            ApiResponse<ViewProduct> createResponse = await _productApiClient.Create(upcreateModel);

            ApiResponse<FileExport> exportResponse = await _productApiClient.Export();

            Assert.That(exportResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(exportResponse.Response.Bytes.Length, Is.GreaterThan(0));
            Assert.IsFalse(string.IsNullOrEmpty(exportResponse.Response.FileName));
            Assert.IsFalse(string.IsNullOrEmpty(exportResponse.Response.ContentType));

            _productIdToDelete = createResponse.Response.Id;
        }
    }
}
