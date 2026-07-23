using ApiClient.Extensions;
using Application.Models.Requests;
using AwesomeAssertions;
using Core.Testing.Builders;
using Core.Testing.Validators;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using System.Net;
using Xunit;
using Serilog.Sinks.InMemory.Assertions;
using IntegrationTests.Collections;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(DevelopmentApiCollection))]
    public class UpdateProductEndpointTests : ProductsTest
    {
        [Fact]
        public async Task UpdatesProductOk()
        {
            //Given
            await CreateProducts();
            var initialProduct = initialProducts[1];

            //When
            var request = new UpdateProductRequestBuilder().Build();
            var response = await ApiClient.UpdateProduct(initialProduct.Id, request);
            var updatedProduct = await response.To<Product>();

            //Then: expected product
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            updatedProduct.Id.Should().BeGreaterThan(0);

            var expected = new ProductBuilder()
                .WithValues(p =>
                {
                    p.Id = initialProduct.Id;
                    p.Name = request.Name;
                    p.Description = request.Description;
                    p.Price = request.Price;
                    p.ImageUrl = updatedProduct.ImageUrl;
                })
                .Build();

            updatedProduct.Should().BeEquivalentTo(expected);
            var updatedProductImage = await ApiClient.HttpClient.GetByteArrayAsync(
                new Uri(updatedProduct.ImageUrl!),
                TestContext.Current.CancellationToken);
            updatedProductImage.Should().BeEquivalentTo(Image2);

            //Then: expected product in db
            var dbProduct = await Context.Products.FindAsync(updatedProduct.Id);
            dbProduct.Should().BeEquivalentTo(expected, o => o.Excluding(p => p.ImageUrl));

            //Then: expected logging
            LogEvents.HaveExactlyOneMessage(
                "Product with id '{id}' updated successfully.",
                LogEventLevel.Information,
                "id", initialProduct.Id);
        }

        [Fact]
        public async Task NoProduct_ExpectedProblemDetails()
        {
            //When
            var request = new UpdateProductRequestBuilder().Build();
            var response = await ApiClient.UpdateProduct(5, request);

            //Then
            await ProblemDetailsValidator.ValidateNotFoundException(response, "Product", "Products", 5);
        }

        [Fact]
        public async Task AllPropertiesInvalid_ExpectedProblemDetails()
        {
            //Given
            await CreateProducts();

            //When
            var request = new UpdateProductRequestBuilder().WithName("").WithDescription("").WithPrice(0m).Build();
            var response = await ApiClient.UpdateProduct(initialProducts[0].Id, request);

            //Then
            await ProblemDetailsValidator.ValidateValidationException(
                response,
                $"{BaseInstance}/{initialProducts[0].Id}",
                new Dictionary<string, string[]>
                {
                    { "name", ["'name' must not be empty."] },
                    { "description", ["'description' must not be empty."] },
                    { "price", ["'price' must be greater than '0'."] }
                });
        }
    }
}