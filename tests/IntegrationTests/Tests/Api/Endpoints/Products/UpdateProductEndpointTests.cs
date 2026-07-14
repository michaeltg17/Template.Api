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
    }
}