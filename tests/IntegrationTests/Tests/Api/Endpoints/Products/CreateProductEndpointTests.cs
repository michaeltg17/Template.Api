using ApiClient.Extensions;
using Application.Features.Images;
using Application.Features.Products.Actions;
using Application.Features.Products.Models.Requests;
using AwesomeAssertions;
using Core.Testing.Assertions;
using Core.Testing.Builders;
using Domain.Models;
using IntegrationTests.Collections;
using IntegrationTests.Extensions;
using IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Serilog.Events;
using Serilog.Sinks.InMemory.Assertions;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(DevelopmentApiCollectionFixture))]
    public class CreateProductEndpointTests(TestFixture testFixture) : ProductsTest(testFixture)
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
            var productImageFileName = ProductService.BuildImageFileName(product, InitialImageExtension);
            var productImageUrl = ImageService.BuildUrl(ImageApiMock.Server.Uri, productImageFileName);
            var expected = new ProductBuilder()
                .WithValues(p =>
                {
                    p.Id = product.Id;
                    p.Name = request.Name;
                    p.Description = request.Description;
                    p.Price = request.Price;
                    p.Image = new Image(productImageFileName, productImageUrl);
                })
                .Build();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            product.Id.Should().BeGreaterThan(0);
            product.Should().BeEquivalentTo(expected);

            //Then: image is uploaded
            ImageApiMock.AssertPostAndSetGetMock(product.Image!.FileName, InitialImage);
            var productImage = await HttpClient
                .GetByteArrayAsync(productImageUrl, TestContext.Current.CancellationToken);
            ImageApiMock.AssertGetRequest(product.Image!.FileName);
            productImage.Should().BeEquivalentTo(InitialImage);

            //Then: expected product in db
            var dbProduct = await Context.Products.FindAsync(product.Id);
            dbProduct.Should().BeEquivalentTo(expected, o => o.Excluding(p => p.Image!.Url));

            //Then: expected logging
            TestFixture.InMemorySink
                .Should()
                .HaveMessage(ProductCreatedMessage)
                .Appearing().Times(4)
                .WithLevel(LogEventLevel.Information)
                .WithProperty("id")
                .WithValues([.. initialProducts.Select(p => p.Id), product.Id]);

            //Then: common expectations
            await AssertCommonExpectations(4, [product.Id]);
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
            await ProblemDetailsAssertions.AssertValidationException(
                response,
                BaseInstance,
                new Dictionary<string, string[]>
                {
                    { "name", ["'name' must not be empty."] },
                    { "description", ["'description' must not be empty."] },
                    { "price", ["'price' must be greater than '0'."] }
                });

            //Then: expected logging
            TestFixture.InMemorySink
                .Should()
                .HaveMessage(ProductCreatedMessage)
                .Appearing().Times(3)
                .WithLevel(LogEventLevel.Information)
                .WithProperty("id")
                .WithValues([.. initialProducts.Select(p => p.Id)]);

            //Then: common expectations
            await AssertCommonExpectations(3);
        }
    }
}