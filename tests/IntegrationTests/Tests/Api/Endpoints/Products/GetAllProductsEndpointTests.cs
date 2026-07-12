using ApiClient.Extensions;
using AwesomeAssertions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(ApiCollection))]
    public class GetAllProductsEndpointTests : Test
    {
        [Fact]
        public async Task EmptyList_ReturnsOk()
        {
            var response = await ApiClient.GetAllProducts();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var products = await response.To<List<Product>>();
            products.Should().BeEmpty();
        }

        [Fact]
        public async Task WithProducts_ReturnsAllInOrder()
        {
            Context.Products.AddRange(
                new Product { Name = "First", Description = "First desc", Price = 10m },
                new Product { Name = "Second", Description = "Second desc", Price = 20m });
            await Context.SaveChangesAsync();

            var response = await ApiClient.GetAllProducts();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var products = await response.To<List<Product>>();
            products.Should().HaveCount(2);
            products[0].Name.Should().Be("First");
            products[1].Name.Should().Be("Second");
        }
    }
}