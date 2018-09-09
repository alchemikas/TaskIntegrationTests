using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Product.Api.Client;
using Product.Api.Contract;
using Product.Api.Contract.Responses;
using Product.Api.IntegrationTest.Models;

namespace Product.Api.IntegrationTest
{
    [TestFixture]
    public class DeleteProductTests
    {
        private readonly IProductApiClient _productApiClient;

        public DeleteProductTests()
        {
            _productApiClient = new ProductApiClient();
        }

        [Test]
        public async Task WhenDeletingExistingProduct_ThenNoContentIsReturned()
        {
            ApiResponse<ViewProduct> createResponse = await _productApiClient.Create(ProductModels.GetCreateModel());

            ApiResponse<Response> deleteResponse = await _productApiClient.Delete(createResponse.Response.Id);

            Assert.That(deleteResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }

        [Test]
        public async Task WhenDeletingNotExistingProduct_ThenBadRequestIsReturned()
        {
            int nonExistingProductId = 999999999;
            
            ApiResponse<Response> deleteResponse = await _productApiClient.Delete(nonExistingProductId);

            Assert.That(deleteResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.IsTrue(deleteResponse.Response.Errors.Any());
        }
    }
}
