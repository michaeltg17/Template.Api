using ApiClient.Extensions;
using Application.Models.Requests;
using AwesomeAssertions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace IntegrationTests.Tests.Api.ProductEndpoints
{
    [Collection(nameof(ApiCollection))]
    public class GetProductTests : Test
    {
        [Fact]
        public async Task ExistingProduct_ReturnsOk()
        {
            var product = new Product { Name = "Test", Description = "A test product", Price = 15m };
            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            var response = await ApiClient.GetProduct(product.Id);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.To<Product>();
            result.Name.Should().Be("Test");
            result.Description.Should().Be("A test product");
            result.Price.Should().Be(15m);
            result.Id.Should().Be(product.Id);
        }

        [Fact]
        public async Task NonExistentProduct_ReturnsNotFound()
        {
            var response = await ApiClient.GetProduct(long.MaxValue);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}