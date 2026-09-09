using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing.Builders;
using IntegrationTests.Collections;
using IntegrationTests.Fixtures;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(DevelopmentApiCollectionFixture))]
    public class UnauthorizedWriteEndpointTests(TestFixture testFixture) : ProductsTest(testFixture)
    {
        [Fact]
        public async Task CreateProductWithoutToken_Expected401()
        {
            var response = await CreateAnonymousApiClient()
                .Products.CreateProduct(new CreateProductRequestBuilder().Build());

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task UpdateProductWithoutToken_Expected401()
        {
            var response = await CreateAnonymousApiClient()
                .Products.UpdateProduct(1L, new UpdateProductRequestBuilder().Build());

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task DeleteProductsWithoutToken_Expected401()
        {
            var response = await CreateAnonymousApiClient()
                .Products.DeleteProducts(new DeleteProductsRequestBuilder().Build());

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetProductsWithoutToken_Expected200()
        {
            var response = await CreateAnonymousApiClient().Products.GetAllProducts();

            await response.ValidateOrThrow(HttpStatusCode.OK);
        }
    }
}
