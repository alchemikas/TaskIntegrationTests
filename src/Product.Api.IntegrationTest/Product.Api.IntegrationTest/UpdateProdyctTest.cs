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
    public class UpdateProdyctTest
    {
        private readonly IProductApiClient _productApiClient;

        public UpdateProdyctTest()
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
        public async Task WhenUpdatingNonExistingProduct_ThenNewProductIsCreated()
        {
            CreateProduct updateModel = ProductModels.GetCreateModel();
            int nonExistingProductId = 9999999;

            ApiResponse<ViewProduct> updateResponse = await _productApiClient.Update(updateModel, nonExistingProductId);

            Assert.That(updateResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(updateResponse.Response.Code, Is.EqualTo(updateModel.Code));
            Assert.That(updateResponse.Response.Name, Is.EqualTo(updateModel.Name));
            Assert.That(updateResponse.Response.Price, Is.EqualTo(updateModel.Price));

            _productIdToDelete = updateResponse.Response.Id;
        }

        [Test]
        public async Task WhenUpdatingExistingProduct_ThenReturnsOk()
        {
            CreateProduct createModel = ProductModels.GetCreateModel();
            ApiResponse<ViewProduct> createResponse = await _productApiClient.Create(createModel);
            Assert.That(createResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.Created));
            _productIdToDelete = createResponse.Response.Id;

            CreateProduct updateModel = ProductModels.GetCreateModel();
            updateModel.Name = "UpdatedName";
            updateModel.Code = "UpdatedCode";


            ApiResponse<ViewProduct> updateResponse = await _productApiClient.Update(updateModel, createResponse.Response.Id);
            Assert.That(updateResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.OK));


            ApiResponse<ProductDetailsResponse> getResponse = await _productApiClient.Get(createResponse.Response.Id);
            Assert.That(getResponse.Response.Product.Id, Is.EqualTo(createResponse.Response.Id));
            Assert.That(getResponse.Response.Product.Name, Is.EqualTo(updateModel.Name));
            Assert.That(getResponse.Response.Product.Code, Is.EqualTo(updateModel.Code));
        }
    }
}
