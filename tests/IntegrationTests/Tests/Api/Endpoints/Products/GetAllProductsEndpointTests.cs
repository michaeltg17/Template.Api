using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing.Builders;
using Domain.Models;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(DevelopmentApiCollection))]
    public class GetAllProductsEndpointTests : ProductsTest
    {
        [Fact]
        public async Task GetProductsOk()
        {
            //Given
            await CreateProducts();

            //When
            var response = await ApiClient.GetAllProducts();

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var products = await response.To<List<Product>>();
            products.Should().BeEquivalentTo(initialProducts, o => o.WithStrictOrdering());
        }

        [Fact]
        public async Task NoProducts_ReturnsOk()
        {
            //When
            var response = await ApiClient.GetAllProducts();

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var products = await response.To<List<Product>>();
            products.Should().BeEmpty();
        }
    }
}