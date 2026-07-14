using ApiClient.Extensions;
using Application.Models.Responses;
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
    public class DeleteProductsEndpointTests : ProductsTest
    {
        [Fact]
        public async Task DeleteSingleOk()
        {
            //Given
            await CreateProducts();

            //When
            var request = new DeleteProductsRequestBuilder()
                .WithIds([initialProducts[1].Id])
                .Build();
            var response = await ApiClient.DeleteProducts(request.Ids, request.IgnoreNotFound);
            var result = await response.To<DeleteProductsResponse>();

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var expected = new DeleteProductsResponseBuilder()
                .WithDeletedIds([initialProducts[1].Id])
                .Build();
            result.Should().BeEquivalentTo(expected);

            var dbProduct = await Context.Products.FindAsync(initialProducts[1].Id);
            dbProduct.Should().BeNull();
            await ValidateInitialProductsAreTheSame([initialProducts[1].Id]);
        }

        [Fact]
        public async Task DeleteMultipleOk()
        {
            //Given
            await CreateProducts();

            //When
            var ids = new[] { initialProducts[0].Id, initialProducts[1].Id };
            var request = new DeleteProductsRequestBuilder()
                .WithIds(ids)
                .Build();
            var response = await ApiClient.DeleteProducts(request.Ids, request.IgnoreNotFound);
            var result = await response.To<DeleteProductsResponse>();

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var expected = new DeleteProductsResponseBuilder()
                .WithDeletedIds(ids)
                .Build();
            result.Should().BeEquivalentTo(expected);

            var dbProducts = await Context.Products.ToListAsync();
            dbProducts.Should().HaveCount(1);
            dbProducts[0].Id.Should().Be(initialProducts[2].Id);
        }

        [Fact]
        public async Task NonExistentProduct_ExpectedProblemDetails()
        {
            //Given
            await CreateProducts();

            //When
            var response = await ApiClient.DeleteProducts([5]);

            //Then
            await ProblemDetailsValidator.ValidateNotFoundException(response, "Product", "Products", [5]);
        }

        [Fact]
        public async Task PartialNotFound_DeleteOnlyFound_OnlyExistingDeleted()
        {
            //Given
            await CreateProducts();

            //When
            var ids = new[] { initialProducts[0].Id, long.MaxValue };
            var request = new DeleteProductsRequestBuilder()
                .WithIds(ids)
                .WithIgnoreNotFound(true)
                .Build();
            var response = await ApiClient.DeleteProducts(request.Ids, request.IgnoreNotFound);
            var result = await response.To<DeleteProductsResponse>();

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var expected = new DeleteProductsResponseBuilder()
                .WithDeletedIds([initialProducts[0].Id])
                .WithNotFoundIds([long.MaxValue])
                .Build();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task AllNotFound_DeleteOnlyFound_ReturnsEmpty()
        {
            //Given
            await CreateProducts();

            //When
            var ids = new[] { long.MaxValue - 1, long.MaxValue };
            var request = new DeleteProductsRequestBuilder()
                .WithIds(ids)
                .WithIgnoreNotFound(true)
                .Build();
            var response = await ApiClient.DeleteProducts(request.Ids, request.IgnoreNotFound);
            var result = await response.To<DeleteProductsResponse>();

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var expected = new DeleteProductsResponseBuilder()
                .WithNotFoundIds(ids)
                .Build();
            result.Should().BeEquivalentTo(expected);
        }
    }
}