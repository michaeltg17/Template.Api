using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing.Builders;
using Domain.Models;
using IntegrationTests.Collections;
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

            //Then: returns products
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var products = await response.To<List<Product>>();
            products.Should().BeEquivalentTo(initialProducts, o => o.WithStrictOrdering());

            //Then: common expectations
            await ValidateCommonExpectations(3);
        }

        [Fact]
        public async Task NoProducts_ReturnsOkEmptyList()
        {
            //When
            var response = await ApiClient.GetAllProducts();

            //Then: returns empty list
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var products = await response.To<List<Product>>();
            products.Should().BeEmpty();
        }
    }
}