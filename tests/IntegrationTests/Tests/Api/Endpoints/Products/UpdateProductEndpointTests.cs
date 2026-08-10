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
using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using Serilog.Sinks.InMemory.Assertions;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(DevelopmentApiCollectionFixture))]
    public class UpdateProductEndpointTests(TestFixture testFixture) : ProductsTest(testFixture)
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
            var product = await response.To<Product>();
            var productImageFileName = ProductService.BuildImageFileName(product, Image2Extension);
            var productImageUrl = ImageService.BuildUrl(ImageApiMock.Server.Uri, productImageFileName);

            //Then: expected product
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            product.Id.Should().BeGreaterThan(0);

            var expected = new ProductBuilder()
                .WithValues(p =>
                {
                    p.Id = initialProduct.Id;
                    p.Name = request.Name;
                    p.Description = request.Description;
                    p.Price = request.Price;
                    p.Image = new Image(productImageFileName, productImageUrl);
                })
                .Build();

            product.Should().BeEquivalentTo(expected);
            ImageApiMock.AssertPostAndSetGetMock($"{product.Id}.jpg", Image2);
            var productImage = await HttpClient.GetByteArrayAsync(productImageUrl);
            productImage.Should().BeEquivalentTo(Image2);
            ImageApiMock.AssertGetRequest($"{product.Id}.jpg");

            //Then: expected product in db
            var dbProduct = await Context.Products.FindAsync(product.Id);
            dbProduct.Should().BeEquivalentTo(expected, o => o.Excluding(p => p.Image!.Url));

            //Then: expected logging
            TestFixture.InMemorySink
                .Should()
                .HaveMessage(ProductUpdatedMessage)
                .Appearing().Once()
                .WithLevel(LogEventLevel.Information)
                .WithProperty("id")
                .WithValue(initialProduct.Id);

            //Then: common expectations
            await AssertCommonExpectations(3, [product.Id]);
        }

        [Fact]
        public async Task NoProduct_ExpectedProblemDetails()
        {
            //Given
            await CreateProducts();

            //When
            var request = new UpdateProductRequestBuilder().Build();
            var response = await ApiClient.UpdateProduct(5, request);

            //Then: product not found
            await ProblemDetailsAssertions.AssertNotFoundException(response, nameof(Product), BaseInstance, 5);

            //Then: expected no logging
            TestFixture.InMemorySink
                .Should()
                .NotHaveMessage(ProductUpdatedMessage);

            //Then: common expectations
            await AssertCommonExpectations(3);
        }

        [Fact]
        public async Task AllPropertiesInvalid_ExpectedProblemDetails()
        {
            //Given
            await CreateProducts();

            //When
            var request = new UpdateProductRequestBuilder().WithName("").WithDescription("").WithPrice(0m).Build();
            var response = await ApiClient.UpdateProduct(initialProducts[0].Id, request);

            //Then: validation exception
            await ProblemDetailsAssertions.AssertValidationException(
                response,
                $"{BaseInstance}/{initialProducts[0].Id}",
                new Dictionary<string, string[]>
                {
                    { "name", ["'name' must not be empty."] },
                    { "description", ["'description' must not be empty."] },
                    { "price", ["'price' must be greater than '0'."] }
                });

            //Then: expected no logging
            TestFixture.InMemorySink
                .Should()
                .NotHaveMessage(ProductUpdatedMessage);

            //Then: common expectations
            await AssertCommonExpectations(3);
        }
    }
}