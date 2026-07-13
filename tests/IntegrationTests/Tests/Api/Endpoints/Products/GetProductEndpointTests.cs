using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing.Builders;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(ApiCollection))]
    public class GetProductEndpointTests : Test
    {
        [Fact]
        public async Task ExistingProduct_ReturnsOk()
        {
            var product = new ProductBuilder().Build();

            await Context.Products.AddAsync(product);
            await Context.SaveChangesAsync();

            var response = await ApiClient.GetProduct(product.Id);
            var result = await response.To<Product>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Id.Should().BeGreaterThan(0);

            var expected = new ProductBuilder()
                .WithValues(p =>
                {
                    p.Id = product.Id;
                    p.Name = product.Name;
                    p.Description = product.Description;
                    p.Price = product.Price;
                })
                .Build();

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task NonExistentProduct_ReturnsNotFound()
        {
            var response = await ApiClient.GetProduct(long.MaxValue);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}