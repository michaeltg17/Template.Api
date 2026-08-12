using ApiClient.Extensions;
using Application.Features.Images;
using Application.Features.Products.Actions;
using AwesomeAssertions;
using Core.Testing.Builders;
using Domain.Models;
using IntegrationTests.Collections;
using IntegrationTests.Extensions;
using IntegrationTests.Fixtures;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(DevelopmentApiCollectionFixture))]
    public class GetAllProductsEndpointTests(TestFixture testFixture) : ProductsTest(testFixture)
    {
        [Fact]
        public async Task GetProductsOk()
        {
            //Given
            await CreateProducts();

            //When
            var response = await ApiClient.GetAllProducts();

            //Then: returns products
            var products = await response.To<List<Product>>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            products.Should().BeEquivalentTo(initialProducts);

            //Then: expected images
            foreach (var product in products)
            {
                var productImageFileName = ProductService.BuildImageFileName(product, InitialImageExtension);
                var productImageUrl = ImageService.BuildUrl(ImageApiMock.Server.Uri, productImageFileName);
                var productImage = await HttpClient.GetByteArrayAsync(productImageUrl);
                productImage.Should()
                    .BeEquivalentTo(InitialImage, $"downloaded image for product '{product.Id}' should match initial image");
                ImageApiMock.AssertGetRequest(productImageFileName);
            }

            //Then: common expectations
            await AssertCommonExpectations(3);
        }

        [Fact]
        public async Task NoProducts_ReturnsOkEmptyList()
        {
            //When
            var response = await ApiClient.GetAllProducts();

            //Then: returns empty list
            var products = await response.To<List<Product>>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            products.Should().BeEmpty();
        }
    }
}