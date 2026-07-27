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

        public List<Product> initialProducts = [];
        internal ImageApiMock ImageApiMock => WebApplicationFactoryFixture.ImageApiMock;

        public async ValueTask CreateProducts()
        {
            var tasks = new[]
            {
                ApiClient.CreateProduct(new CreateProductRequestBuilder().Build()).To<Product>(),
                ApiClient.CreateProduct(new CreateProductRequestBuilder().Build()).To<Product>(),
                ApiClient.CreateProduct(new CreateProductRequestBuilder().Build()).To<Product>()
            };
            initialProducts.AddRange((await Task.WhenAll(tasks)).OrderBy(p => p.Id));

            foreach (var product in initialProducts)
            {
                ImageApiMock.SetGetMock(product.ImageName!, InitialImage);
            }
        }

        public async Task ValidateCommonExpectations(int totalProductsCount, IEnumerable<long>? exceptIds = null)
        {
            var productsToValidate = exceptIds == null
                ? initialProducts
                : [.. initialProducts.Where(p => !exceptIds.Contains(p.Id))];

            //Expected products in db
            var dbProducts = await Context.Products.ToListAsync();
            dbProducts.Where(p => !exceptIds.Contains(p.Id))
                .Should().BeEquivalentTo(productsToValidate, o => o.Excluding(p => p.ImageUrl));
            dbProducts.Count.Should().Be(totalProductsCount);
        }
    }
}