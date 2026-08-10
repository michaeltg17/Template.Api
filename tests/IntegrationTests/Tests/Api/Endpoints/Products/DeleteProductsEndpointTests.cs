using ApiClient.Extensions;
using Application.Features.Products.Actions;
using Application.Features.Products.Models.Requests;
using Application.Features.Products.Models.Responses;
using AwesomeAssertions;
using Core.Testing.Assertions;
using Domain.Models;
using IntegrationTests.Collections;
using IntegrationTests.Extensions;
using IntegrationTests.Fixtures;
using Serilog.Events;
using Serilog.Sinks.InMemory.Assertions;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(DevelopmentApiCollectionFixture))]
    public class DeleteProductsEndpointTests(TestFixture testFixture) : ProductsTest(testFixture)
    {
        [Fact]
        public async Task DeleteSingleOk()
        {
            //Given
            await CreateProducts();
            var product = initialProducts[1];
            var request = new DeleteProductsRequest([product.Id]);

            //When
            var response = await ApiClient.DeleteProducts(request);

            //Then: expected response
            var result = await response.To<DeleteProductsResponse>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var expected = new DeleteProductsResponse([product.Id], []);
            result.Should().BeEquivalentTo(expected);

            //Then: expected logging
            TestFixture.InMemorySink
                .Should()
                .HaveMessage(ProductsDeletedMessage)
                .Appearing().Once()
                .WithLevel(LogEventLevel.Information)
                .WithProperty("ids")
                .WithValues([product.Id]);

            //Then: expected image delete
            ImageApiMock.AssertDeleteRequests([product.Image!.FileName]);

            //Then: common expectations
            await AssertCommonExpectations(2, [product.Id]);
        }

        [Fact]
        public async Task DeleteMultipleOk()
        {
            //Given
            await CreateProducts(5);
            var products = new[] { initialProducts[0], initialProducts[1], initialProducts[4] };
            var ids = products.Select(p => p.Id).ToList();
            var request = new DeleteProductsRequest(ids);

            //When
            var response = await ApiClient.DeleteProducts(request);

            //Then: expected response
            var result = await response.To<DeleteProductsResponse>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var expected = new DeleteProductsResponse(ids, []);
            result.Should().BeEquivalentTo(expected);

            //Then: expected logging
            TestFixture.InMemorySink
                .Should()
                .HaveMessage(ProductsDeletedMessage)
                .Appearing().Once()
                .WithLevel(LogEventLevel.Information)
                .WithProperty("ids")
                .WithValues(ids);

            //Then: expected image deletes
            ImageApiMock.AssertDeleteRequests(products.Select(p => p.Image!.FileName));

            //Then: common expectations
            await AssertCommonExpectations(2, ids);
        }

        [Fact]
        public async Task NoProducts_IgnoreNotFoundFalse_ExpectedProblemDetails()
        {
            //Given
            await CreateProducts();
            var request = new DeleteProductsRequest([5, 6]);

            //When
            var response = await ApiClient.DeleteProducts(request);

            //Then
            await ProblemDetailsAssertions.AssertNotAllFoundException(response, nameof(Product), BaseInstance, [5, 6]);
        }

        [Fact]
        public async Task SomeNotFound_IgnoreNotFoundTrue_ExistingDeleted()
        {
            //Given
            await CreateProducts();
            var existingId = initialProducts[0].Id;
            var notFoundId = 10;
            var request = new DeleteProductsRequest([existingId, notFoundId], true);

            //When
            var response = await ApiClient.DeleteProducts(request);

            //Then: expected response
            var result = await response.To<DeleteProductsResponse>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var expected = new DeleteProductsResponse([existingId], [notFoundId]);
            result.Should().BeEquivalentTo(expected);

            //Then: expected logging
            TestFixture.InMemorySink
                .Should()
                .HaveMessage(ProductsDeletedMessage)
                .Appearing().Once()
                .WithLevel(LogEventLevel.Information)
                .WithProperty("ids")
                .WithValues([existingId]);

            //Then: expected image delete
            ImageApiMock.AssertDeleteRequests([initialProducts[0].Image!.FileName]);

            //Then: common expectations
            await AssertCommonExpectations(2, [existingId]);
        }

        [Fact]
        public async Task AllNotFoundIds_IgnoreNotFoundTrue_ExpectedResponse()
        {
            //Given
            await CreateProducts();
            long[] ids = [15, 16];
            var request = new DeleteProductsRequest(ids, true);

            //When
            var response = await ApiClient.DeleteProducts(request);

            //Then: expected response
            var result = await response.To<DeleteProductsResponse>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var expected = new DeleteProductsResponse([], ids);
            result.Should().BeEquivalentTo(expected);

            //Then: expected logging
            TestFixture.InMemorySink
                .Should()
                .NotHaveMessage(ProductsDeletedMessage);

            //Then: common expectations
            await AssertCommonExpectations(3);
        }
    }
}