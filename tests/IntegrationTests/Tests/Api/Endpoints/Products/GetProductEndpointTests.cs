using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing.Builders;
using Core.Testing.Validators;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(DevelopmentApiCollection))]
    public class GetProductEndpointTests : ProductsTest
    {
        [Fact]
        public async Task GetProductOk()
        {
            //Given
            await CreateProducts();
            var product = initialProducts[1];

            //When
            var response = await ApiClient.GetProduct(product.Id);

            //Then
            var result = await response.To<Product>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var expected = new ProductBuilder()
                .WithValues(p =>
                {
                    p.Id = product.Id;
                    p.Name = product.Name;
                    p.Description = product.Description;
                    p.Price = product.Price;
                })
                .Build();

            result.Id.Should().BeGreaterThan(0);
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task NoProduct_ExpectedProblemDetails()
        {
            //When
            var response = await ApiClient.GetProduct(1);

            //Then
            await ProblemDetailsValidator.ValidateNotFoundException(response, "Product", "Products", 1);
        }
    }
}