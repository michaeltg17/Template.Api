using ApiClient.Extensions;
using Application.Features.Images;
using AwesomeAssertions;
using Core.Testing.Builders;
using Core.Testing.Validators;
using Domain.Models;
using IntegrationTests.Collections;
using IntegrationTests.Extensions;
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
            var initialProduct = initialProducts[1];

            //When
            var response = await ApiClient.GetProduct(initialProduct.Id);

            //Then
            var product = await response.To<Product>();

            var expected = new ProductBuilder()
                .WithValues(p =>
                {
                    p.Id = initialProduct.Id;
                    p.Name = initialProduct.Name;
                    p.Description = initialProduct.Description;
                    p.Price = initialProduct.Price;
                    p.ImageName = product.ImageName;
                    p.ImageUrl = product.ImageUrl;
                })
                .Build();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            product.Id.Should().BeGreaterThan(0);
            product.Should().BeEquivalentTo(expected);

            //Verify uploads and register GET stub
            ImageApiMock.ValidatePostAndSetGetMock(initialProduct.ImageName, InitialImage);
            var productImageUrl = ImageService.BuildUrl(ImageApiMock.Server.Uri, product.ImageName!);
            var productImage = await ImageHttpClient.GetByteArrayAsync(productImageUrl);
            productImage.Should().BeEquivalentTo(InitialImage);
            ImageApiMock.ValidateGetRequest(initialProduct.ImageName);

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
            await ProblemDetailsValidator.ValidateNotFoundException(response, nameof(Product), BaseInstance, 4);

            //Then: common expectations
            await ValidateCommonExpectations(3);
        }
    }
}