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
    public class UpdateProductEndpointTests : ProductsTest
    {
        [Fact]
        public async Task UpdatesProductOk()
        {
            //Given
            await CreateProducts();
            var product = initialProducts[1];

            //When
            var request = new UpdateProductRequestBuilder().Build();
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
        public async Task NoProduct_ExpectedProblemDetails()
        {
            //When
            var request = new UpdateProductRequestBuilder().Build();
            var response = await ApiClient.UpdateProduct(5, request);

            //Then
            await ProblemDetailsValidator.ValidateNotFoundException(response, "Product", "Products", 5);
        }

        [Fact]
        public async Task InvalidName_ExpectedProblemDetails()
        {
            //Given
            await CreateProducts();

            //When
            var request = new UpdateProductRequestBuilder().WithName("").Build();
            var response = await ApiClient.UpdateProduct(initialProducts[1].Id, request);

            //Then
            await ProblemDetailsValidator.ValidateValidationException(
                response,
                $"{BaseInstance}/{initialProducts[1].Id}",
                "Validation failed: \r\n -- Name: 'Name' must not be empty. Severity: Error");
        }

        [Fact]
        public async Task InvalidDescription_ExpectedProblemDetails()
        {
            //Given
            await CreateProducts();

            //When
            var request = new UpdateProductRequestBuilder().WithDescription("").Build();
            var response = await ApiClient.UpdateProduct(initialProducts[0].Id, request);

            //Then
            await ProblemDetailsValidator.ValidateValidationException(
                response,
                $"{BaseInstance}/{initialProducts[0].Id}",
                "Validation failed: \r\n -- Description: 'Description' must not be empty. Severity: Error");
        }

        [Fact]
        public async Task InvalidPrice_ExpectedProblemDetails()
        {
            //Given
            await CreateProducts();

            //When
            var request = new UpdateProductRequestBuilder().WithPrice(0m).Build();
            var response = await ApiClient.UpdateProduct(initialProducts[2].Id, request);

            //Then
            await ProblemDetailsValidator.ValidateValidationException(
                response,
                $"{BaseInstance}/{initialProducts[2].Id}",
                "Validation failed: \r\n -- Price: 'Price' must be greater than '0'. Severity: Error");
        }
    }
}