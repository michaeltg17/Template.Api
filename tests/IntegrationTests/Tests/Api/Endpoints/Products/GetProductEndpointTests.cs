using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing.Builders;
using Core.Testing.Validators;
using Domain.Models;
using IntegrationTests.Collections;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;
using static System.Net.Mime.MediaTypeNames;

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
            var initialProduct = initialProducts[1];

            //When
            var response = await ApiClient.GetProduct(initialProduct.Id);

            //Then
            var product = await response.To<Product>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var expected = new ProductBuilder()
                .WithValues(p =>
                {
                    p.Id = initialProduct.Id;
                    p.Name = initialProduct.Name;
                    p.Description = initialProduct.Description;
                    p.Price = initialProduct.Price;
                    p.ImageUrl = product.ImageUrl;
                })
                .Build();

            product.Id.Should().BeGreaterThan(0);
            product.Should().BeEquivalentTo(expected);
            var productImage = await ApiClient.HttpClient.GetByteArrayAsync(
                new Uri(product.ImageUrl!),
                TestContext.Current.CancellationToken);
            productImage.Should().BeEquivalentTo(InitialImage);

            //Then: common expectations
            await ValidateCommonExpectations(3);
        }

        [Fact]
        public async Task NoProduct_ExpectedProblemDetails()
        {
            //Given
            await CreateProducts();

            //When
            var response = await ApiClient.GetProduct(4);

            //Then: product not found
            await ProblemDetailsValidator.ValidateNotFoundException(response, "Product", "Products", 4);

            //Then: common expectations
            await ValidateCommonExpectations(3);
        }
    }
}