using AwesomeAssertions;
using Core.Testing.Builders;
using Core.Testing.Validators;
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
            await Context.Products.AddAsync(product);
            await Context.SaveChangesAsync();

            //When
            var response = await ApiClient.DeleteProduct(product.Id);

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            Context.ChangeTracker.Clear();
            var dbProduct = await Context.Products.FindAsync(product.Id);
            dbProduct.Should().BeNull();
        }

        [Fact]
        public async Task NonExistentProduct_ExpectedProblemDetails()
        {
            //When
            var response = await ApiClient.DeleteProduct(1);

            //Then
            await ProblemDetailsValidator.ValidateNotFoundException(response, "Product", "Products", 1);
        }
    }
}