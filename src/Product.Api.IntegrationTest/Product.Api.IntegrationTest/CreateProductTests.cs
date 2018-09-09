using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Product.Api.Client;
using Product.Api.Contract;
using Product.Api.IntegrationTest.Models;

namespace Product.Api.IntegrationTest
{
    [TestFixture]
    public class CreateProductTests
    {

        private readonly IProductApiClient _productApiClient;

        public CreateProductTests()
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
        public async Task WhenCreatingValidProduct_ThenCreatedIsReturned()
        {
            CreateProduct productModel = ProductModels.GetCreateModel();

            ApiResponse<ViewProduct> apiResponse = await _productApiClient.Create(productModel);

            Assert.That(apiResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(apiResponse.Response.Code, Is.EqualTo(productModel.Code));
            Assert.That(apiResponse.Response.Name, Is.EqualTo(productModel.Name));
            Assert.That(apiResponse.Response.Price, Is.EqualTo(productModel.Price));
            Assert.That(apiResponse.Response.Photo.Content, Is.EqualTo(productModel.Photo.Content));
            Assert.That(apiResponse.Response.Photo.ContentType, Is.EqualTo(productModel.Photo.ContentType));
            Assert.That(apiResponse.Response.Photo.Title, Is.EqualTo(productModel.Photo.Title));

            _productIdToDelete = apiResponse.Response.Id;
        }

        [Test]
        public async Task WhenCreatingProductWithNonUniqueCode_ThenBadRequestIsReturned()
        {
            CreateProduct productModel = ProductModels.GetCreateModel();
            ApiResponse<ViewProduct> apiResponse = await _productApiClient.Create(productModel);

            ApiResponse<ViewProduct> createResponse = await _productApiClient.Create(productModel);

            Assert.That(createResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.IsTrue(createResponse.Response.Errors.Any());

            _productIdToDelete = apiResponse.Response.Id;
        }

        [Test]
        public async Task WhenCreatingProductWithoutName_ThenBadRequestIsReturned()
        {
            CreateProduct productModel = ProductModels.GetCreateModel();
            productModel.Name = String.Empty;

            ApiResponse<ViewProduct> createResponse = await _productApiClient.Create(productModel);

            Assert.That(createResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.IsTrue(createResponse.Response.Errors.Any());
        }

        [Test]
        public async Task WhenCreatingProductWhenPriceNotInRange_ThenBadRequestIsReturned()
        {
            CreateProduct productModel = ProductModels.GetCreateModel();
            productModel.Price = 9999999m;

            ApiResponse<ViewProduct> createResponse = await _productApiClient.Create(productModel);

            Assert.That(createResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.IsTrue(createResponse.Response.Errors.Any());
        }

        [Test]
        public async Task WhenCreatingProductWithoutImage_ThenCreatedIsReturned()
        {
            CreateProduct productModel = ProductModels.GetCreateModel();
            productModel.Photo = null;

            ApiResponse<ViewProduct> apiResponse = await _productApiClient.Create(productModel);

            Assert.That(apiResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(apiResponse.Response.Code, Is.EqualTo(productModel.Code));
            Assert.That(apiResponse.Response.Name, Is.EqualTo(productModel.Name));
            Assert.That(apiResponse.Response.Price, Is.EqualTo(productModel.Price));
            Assert.IsNull(apiResponse.Response.Photo);

            _productIdToDelete = apiResponse.Response.Id;
        }
    }
}
