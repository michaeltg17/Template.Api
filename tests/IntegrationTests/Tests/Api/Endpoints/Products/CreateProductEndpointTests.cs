using ApiClient.Extensions;
using Application.Features.Products.Models.Requests;
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
            await CreateProducts();
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
                    p.ImageName = product.ImageName;
                    p.ImageUrl = product.ImageUrl;
                })
                .Build();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            product.Id.Should().BeGreaterThan(0);

            //Verify uploads and register GET stubs before downloading
            foreach (var p in initialProducts)
                ImageApiMock.VerifyUploadAndStore($"{p.Id}.jpeg", InitialImage);
            ImageApiMock.VerifyUploadAndStore($"{product.Id}.jpeg", InitialImage);

            var productImageUrl = $"{ImageApiMock.Url}/api/v1/images/{product.Id}.jpeg";
            var productImage = await ImageHttpClient.GetByteArrayAsync(productImageUrl);
            productImage.Should().BeEquivalentTo(InitialImage);
            ImageApiMock.VerifyDownload($"{product.Id}.jpeg");
            product.Should().BeEquivalentTo(expected);

            //Then: expected product in db
            var dbProduct = await Context.Products.FindAsync(product.Id);
            dbProduct.Should().BeEquivalentTo(expected, o => o.Excluding(p => p.ImageUrl));

            //Then: expected logging
            WebApplicationFactoryFixture.InMemorySink
                .Should()
                .HaveMessage("Product with id '{id}' created successfully.")
                .Appearing().Times(4)
                .WithLevel(LogEventLevel.Information)
                .WithProperty("id")
                .WithValues([.. initialProducts.Select(p => p.Id), product.Id]);

            //Then: common expectations
            await ValidateCommonExpectations(4, [product.Id]);
        }

        [Fact]
        public async Task AllPropertiesInvalid_ExpectedProblemDetails()
        {
            //Given
            await CreateProducts();

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

            //Then: expected logging
            WebApplicationFactoryFixture.InMemorySink
                .Should()
                .HaveMessage("Product with id '{id}' created successfully.")
                .Appearing().Times(3)
                .WithLevel(LogEventLevel.Information)
                .WithProperty("id")
                .WithValues([.. initialProducts.Select(p => p.Id)]);

            //Then: common expectations
            await ValidateCommonExpectations(3);
        }
    }
}