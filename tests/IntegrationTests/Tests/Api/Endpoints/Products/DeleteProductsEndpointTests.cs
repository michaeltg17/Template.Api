using ApiClient.Extensions;
using Application.Features.Products.Models.Requests;
using AwesomeAssertions;
using Core.Testing.Builders;
using Core.Testing.Extensions;
using Core.Testing.Validators;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using Serilog.Events;
using System.Net;
using Xunit;
using Serilog.Sinks.InMemory.Assertions;
using IntegrationTests.Collections;
using Application.Features.Products.Models.Responses;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(DevelopmentApiCollection))]
    public class DeleteProductsEndpointTests : ProductsTest
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
            WebApplicationFactoryFixture.InMemorySink
                .Should()
                .HaveMessage("Products with ids '{ids}' deleted successfully.")
                .Appearing().Once()
                .WithLevel(LogEventLevel.Information)
                .WithProperty("ids")
                .WithValue([product.Id]);

            //Then: expected image delete
            ImageApiMock.ValidateDeleteRequests([product.ImageName!]);

            //Then: common expectations
            await ValidateCommonExpectations(2, [product.Id]);
        }

        [Fact]
        public async Task DeleteMultipleOk()
        {
            //Given
            await CreateProducts();
            var ids = new[] { initialProducts[0].Id, initialProducts[1].Id };
            var request = new DeleteProductsRequest(ids);

            //When
            var response = await ApiClient.DeleteProducts(request);

            //Then: expected response
            var result = await response.To<DeleteProductsResponse>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var expected = new DeleteProductsResponse(ids, []);
            result.Should().BeEquivalentTo(expected);

            //Then: expected logging
            LogEventPropertyAssertionExtensions.WithValue(WebApplicationFactoryFixture.InMemorySink
                .Should()
                .HaveMessage("Products with ids '{ids}' deleted successfully.")
                .Appearing().Once()
                .WithLevel(LogEventLevel.Information)
                .WithProperty("ids")
                , ids);

            //Then: expected image deletes
            ImageApiMock.ValidateDeleteRequests([initialProducts[0].ImageName!, initialProducts[1].ImageName!]);

            //Then: common expectations
            await ValidateCommonExpectations(1, ids);
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
            await ProblemDetailsValidator.ValidateNotAllFoundException(response, nameof(Product), BaseInstance, [5, 6]);
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
            WebApplicationFactoryFixture.InMemorySink
                .Should()
                .HaveMessage("Products with ids '{ids}' deleted successfully.")
                .Appearing().Once()
                .WithLevel(LogEventLevel.Information)
                .WithProperty("ids")
                .WithValue([existingId]);

            //Then: expected image delete
            ImageApiMock.ValidateDeleteRequests([$"{existingId}.jpeg"]);

            //Then: common expectations
            await ValidateCommonExpectations(2, [existingId]);
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
            WebApplicationFactoryFixture.InMemorySink
                .Should()
                .NotHaveMessage("Products with ids '{ids}' deleted successfully.");

            //Then: common expectations
            await ValidateCommonExpectations(3);
        }
    }
}