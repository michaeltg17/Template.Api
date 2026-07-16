using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing.Builders;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    public abstract class ProductsTest : Test
    {
        protected const string BaseInstance = "/api/Products";

        public List<Product> initialProducts = new();

        public async ValueTask CreateProducts()
        {
            var request = new CreateProductRequestBuilder().Build();
            var tasks = new[]
            {
                ApiClient.CreateProduct(request).To<Product>(),
                ApiClient.CreateProduct(request).To<Product>(),
                ApiClient.CreateProduct(request).To<Product>()
            };
            initialProducts.AddRange((await Task.WhenAll(tasks)).OrderBy(p => p.Id));
        }

        public async Task ValidateInitialProductsAreTheSame(IEnumerable<long>? exceptIds = null)
        {
            exceptIds ??= Array.Empty<long>();
            var dbProducts = await Context.Products.ToListAsync();
            var expectedProducts = initialProducts.Where(p => !exceptIds.Contains(p.Id)).ToList();
            dbProducts.Should().BeEquivalentTo(expectedProducts);
        }
    }
}
