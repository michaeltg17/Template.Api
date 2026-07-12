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
        public async Task DeleteOk()
        {
            //Given
            var product = new ProductBuilder().Build();
            Context.Products.Add(product);
            await Context.SaveChangesAsync();

            //When
            var response = await ApiClient.DeleteProduct(product.Id);

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            var dbProduct = await Context.Products.FindAsync(product.Id);
            dbProduct.Should().BeNull();
        }

        [Fact]
        public async Task NonExistentProduct_ExpectedProblemDetails()
        {
            var response = await ApiClient.DeleteProduct(long.MaxValue);
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}