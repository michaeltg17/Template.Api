using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing.Builders;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using IntegrationTests.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    public abstract class ProductsTest : Test
    {
        protected const string BaseInstance = "/api/Products";
        protected static byte[] InitialImage = File.ReadAllBytes("Images/didi.jpeg");
        protected static byte[] Image2 = File.ReadAllBytes("Images/didi2.jpg");

        public List<Product> initialProducts = new();
        protected ImageApiMock ImageMockServer => WebApplicationFactoryFixture.ImageApiMock;

        public async ValueTask CreateProducts()
        {
            var tasks = new[]
            {
                ApiClient.CreateProduct(new CreateProductRequestBuilder().Build()).To<Product>(),
                ApiClient.CreateProduct(new CreateProductRequestBuilder().Build()).To<Product>(),
                ApiClient.CreateProduct(new CreateProductRequestBuilder().Build()).To<Product>()
            };
            initialProducts.AddRange((await Task.WhenAll(tasks)).OrderBy(p => p.Id));
        }

        public async Task ValidateCommonExpectations(int totalProductsCount, IEnumerable<long>? exceptIds = null)
        {
            exceptIds ??= [];
            var productsToValidate = initialProducts.Where(p => !exceptIds.Contains(p.Id)).ToList();

            //Expected products in db
            var dbProducts = await Context.Products.ToListAsync();
            dbProducts.Where(p => !exceptIds.Contains(p.Id)).Should().BeEquivalentTo(productsToValidate, o => o.Excluding(p => p.ImageUrl));
            dbProducts.Count.Should().Be(totalProductsCount);

            //Expected image uploads
            foreach (var product in productsToValidate)
            {
                var imageBytes = ImageMockServer.VerifyUploadAndStore($"{product.Id}.jpeg", InitialImage);
                imageBytes.Should().BeEquivalentTo(InitialImage, $"uploaded image for product '{product.Id}' should match initial image");
            }
        }
    }
}