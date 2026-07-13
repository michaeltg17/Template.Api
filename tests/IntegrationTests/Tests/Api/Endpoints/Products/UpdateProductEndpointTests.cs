using ApiClient.Extensions;
using Application.Models.Requests;
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
    public class UpdateProductEndpointTests : Test
    {
        [Fact]
        public async Task UpdatesProductOk()
        {
            //Given
            var product = new ProductBuilder().Build();
            await Context.Products.AddAsync(product);
            await Context.SaveChangesAsync();

            //When
            var request = new UpdateProductRequest("Updated", "Updated desc", 20.50m);
            var response = await ApiClient.UpdateProduct(product.Id, request);
            var updatedProduct = await response.To<Product>();

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            updatedProduct.Id.Should().BeGreaterThan(0);

            var expected = new ProductBuilder()
                .WithValues(p =>
                {
                    p.Id = product.Id;
                    p.Name = request.Name;
                    p.Description = request.Description;
                    p.Price = request.Price;
                })
                .Build();

            updatedProduct.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task NonExistentProduct_ExpectedProblemDetails()
        {
            //When
            var request = new UpdateProductRequest("Updated", "Desc", 10m);
            var response = await ApiClient.UpdateProduct(long.MaxValue, request);

            //Then
            await ProblemDetailsValidator.ValidateNotFoundException(response!, "Product", "Products", long.MaxValue);
        }
    }
}