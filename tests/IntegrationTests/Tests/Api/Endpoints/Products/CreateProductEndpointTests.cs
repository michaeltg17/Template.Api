using ApiClient.Extensions;
using Application.Models.Requests;
using AwesomeAssertions;
using Core.Testing.Builders;
using Core.Testing.Validators;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using System.Net;
using Xunit;
using Serilog.Sinks.InMemory.Assertions;
using IntegrationTests.Collections;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(DevelopmentApiCollection))]
    public class CreateProductEndpointTests : ProductsTest
    {
        [Fact]
        public async Task CreateProductOk()
        {
            //Given
            var request = new CreateProductRequestBuilder().Build();

            //When
            var response = await ApiClient.CreateProduct(request);

            //Then: retuns expected product
            var product = await response.To<Product>();
            var expected = new ProductBuilder()
                .WithValues(p =>
                {
                    p.Id = product.Id;
                    p.Name = request.Name;
                    p.Description = request.Description;
                    p.Price = request.Price;
                    p.ImageUrl = product.ImageUrl;
                })
                .Build();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            product.Id.Should().BeGreaterThan(0);
            product.Should().BeEquivalentTo(expected);
            var productImage = await ApiClient.HttpClient.GetByteArrayAsync(
                new Uri(product.ImageUrl!),
                TestContext.Current.CancellationToken);
            productImage.Should().BeEquivalentTo(InitialImage);

            //Then: expected product in db
            var dbProduct = await Context.Products.FindAsync(product.Id);
            dbProduct.Should().BeEquivalentTo(expected, o => o.Excluding(p => p.ImageUrl));

            //Then: expected logging
            LogEvents.HaveExactlyOneMessage(
                "Product with id '{id}' created successfully.",
                LogEventLevel.Information,
                "id", product.Id);
        }

        [Fact]
        public async Task AllPropertiesInvalid_ExpectedProblemDetails()
        {
            //When
            var request = new CreateProductRequestBuilder().WithName("").WithDescription("").WithPrice(0m).Build();
            var response = await ApiClient.CreateProduct(request);

            //Then
            await ProblemDetailsValidator.ValidateValidationException(
                response,
                BaseInstance,
                new Dictionary<string, string[]>
                {
                    { "name", ["'name' must not be empty."] },
                    { "description", ["'description' must not be empty."] },
                    { "price", ["'price' must be greater than '0'."] }
                });
        }
    }
}