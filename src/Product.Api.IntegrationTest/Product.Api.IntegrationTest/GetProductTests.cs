using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Product.Api.Client;
using Product.Api.Contract;
using Product.Api.Contract.Responses;
using Product.Api.IntegrationTest.Models;

namespace Product.Api.IntegrationTest
{
    [TestFixture]
    public class GetProductTests
    {
        private readonly IProductApiClient _productApiClient;

        public GetProductTests()
        {
            _productApiClient = new ProductApiClient();
        }

        CreateProduct productModel = ProductModels.GetCreateModel();
        private int _createdProductId = default(int);

        [SetUp]
        public async Task Setup()
        {
            ApiResponse<ViewProduct> apiResponse  = await _productApiClient.Create(productModel);
            _createdProductId = apiResponse.Response.Id;
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_createdProductId != default(int))
            {
                await _productApiClient.Delete(_createdProductId);
            }
        }

        [Test]
        public async Task WhenGetingList_ThenListIsReturned()
        {
            var productsList = await _productApiClient.GetAll(string.Empty);

            Assert.That(productsList.Response.Products.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(productsList.Response.Products.Any(x => x.Name == productModel.Name), Is.True);
            Assert.That(productsList.Response.Errors.Any(), Is.False);
        }

        [Test]
        public async Task WhenGetingListWithSearchTerm_ThenProductsWithSearchTermReturned()
        {
            var searchCode = productModel.Code;

            var productsList = await _productApiClient.GetAll(searchCode);

            Assert.That(productsList.Response.Products.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(productsList.Response.Products.Any(x => x.Code == searchCode), Is.True);
            Assert.That(productsList.Response.Errors.Any(), Is.False);
        }

        [Test]
        public async Task WhenGetingSingleProduct_ThenProductIsReturned()
        {
            int productId = _createdProductId;

            var product = await _productApiClient.Get(productId);

            Assert.That(product.Response.Product, Is.Not.Null);
            Assert.That(product.Response.Product.Id, Is.EqualTo(productId));
            Assert.That(product.Response.Product.Name, Is.EqualTo(productModel.Name));
            Assert.That(product.Response.Product.Code, Is.EqualTo(productModel.Code));
            Assert.That(product.Response.Product.Price, Is.EqualTo(productModel.Price));
        }
    }
}
