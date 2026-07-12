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
    public class UpdateProductTests : Test
    {
        [Fact]
        public async Task ExistingProduct_ReturnsUpdated()
        {
            var product = new Product { Name = "Original", Description = "Original desc", Price = 10m };
            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            var request = new UpdateProductRequest("Updated", "Updated desc", 20.50m);
            var response = await ApiClient.UpdateProduct(product.Id, JsonContent.Create(request));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updated = await response.To<Product>();
            updated.Name.Should().Be("Updated");
            updated.Description.Should().Be("Updated desc");
            updated.Price.Should().Be(20.50m);
            updated.Id.Should().Be(product.Id);
        }

        [Fact]
        public async Task NonExistentProduct_ReturnsNotFound()
        {
            var request = new UpdateProductRequest("Updated", "Desc", 10m);
            var response = await ApiClient.UpdateProduct(long.MaxValue, JsonContent.Create(request));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task NoBody_ReturnsBadRequest()
        {
            var product = new Product { Name = "Test", Description = "Desc", Price = 10m };
            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            var response = await ApiClient.UpdateProduct(product.Id, null);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}