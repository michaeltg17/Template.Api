using ApiClient.Extensions;
using Application.Features.Images;
using Application.Features.Products.Actions;
using AwesomeAssertions;
using Core.Testing.Assertions;
using Core.Testing.Builders;
using Domain.Models;
using IntegrationTests.Collections;
using IntegrationTests.Extensions;
using IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(DevelopmentApiCollectionFixture))]
    public class GetProductEndpointTests(TestFixture testFixture) : ProductsTest(testFixture)
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
            var productImageFileName = ProductService.BuildImageFileName(product, InitialImageExtension);
            var productImageUrl = ImageService.BuildUrl(ImageApiMock.Server.Uri, productImageFileName);

            var expected = new ProductBuilder()
                .WithValues(p =>
                {
                    p.Id = initialProduct.Id;
                    p.Name = initialProduct.Name;
                    p.Description = initialProduct.Description;
                    p.Price = initialProduct.Price;
                    p.Image = new Image(productImageFileName, productImageUrl);
                })
                .Build();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            product.Id.Should().BeGreaterThan(0);
            product.Should().BeEquivalentTo(expected);

            //Verify uploads and register GET stub
            ImageApiMock.AssertPostAndSetGetMock(initialProduct.Image!.FileName, InitialImage);
            var productImage = await HttpClient.GetByteArrayAsync(productImageUrl);
            productImage.Should().BeEquivalentTo(InitialImage);
            ImageApiMock.AssertGetRequest(initialProduct.Image!.FileName);

            //Then: common expectations
            await AssertCommonExpectations(3);
        }

        [Fact]
        public async Task NoProduct_ExpectedProblemDetails()
        {
            //Given
            await CreateProducts();

            //When
            var response = await ApiClient.GetProduct(4);

            //Then: product not found
            await ProblemDetailsAssertions.AssertNotFoundException(response, nameof(Product), BaseInstance, 4);

            //Then: common expectations
            await AssertCommonExpectations(3);
        }
    }
}