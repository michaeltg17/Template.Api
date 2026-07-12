using AwesomeAssertions;
using Core.Testing.Builders;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(ApiCollection))]
    public class DeleteProductEndpointTests : Test
    {
        [Fact]
        public async Task ExistingProduct_ReturnsNoContent()
        {
            var product = new ProductBuilder().Build();
            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            var response = await ApiClient.DeleteProduct(product.Id);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task ExistingProduct_RemovedFromDatabase()
        {
            var product = new ProductBuilder().Build();
            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            await ApiClient.DeleteProduct(product.Id);

            var dbProduct = await Context.Products.FindAsync(product.Id);
            dbProduct.Should().BeNull();
        }

        [Fact]
        public async Task NonExistentProduct_ReturnsNoContent()
        {
            var response = await ApiClient.DeleteProduct(long.MaxValue);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}