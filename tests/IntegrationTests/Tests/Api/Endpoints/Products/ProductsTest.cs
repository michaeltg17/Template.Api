using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing.Builders;
using Domain.Models;
using IntegrationTests.Fixtures;
using IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    public abstract class ProductsTest(TestFixture testFixture) : Test(testFixture)
    {
        protected const string BaseInstance = "/api/Products";
        protected static byte[] InitialImage = File.ReadAllBytes("Images/didi.jpeg");
        protected static string InitialImageExtension = Path.GetExtension("didi.jpeg");
        protected static byte[] Image2 = File.ReadAllBytes("Images/didi2.jpg");
        protected static string Image2Extension = Path.GetExtension("didi2.jpg");

        protected const string ProductCreatedMessage = "Product with id '{id}' created successfully.";
        protected const string ProductUpdatedMessage = "Product with id '{id}' updated successfully.";
        protected const string ProductsDeletedMessage = "Products with ids '{ids}' deleted successfully.";

        public List<Product> initialProducts = [];
        internal ImageApiMock ImageApiMock => TestFixture.ImageApiMock;

        public async ValueTask CreateProducts(int count = 3)
        {
            var tasks = Enumerable
                .Range(0, count)
                .Select(_ => ApiClient.CreateProduct(new CreateProductRequestBuilder().Build()).To<Product>());

            initialProducts.AddRange((await Task.WhenAll(tasks)).OrderBy(p => p.Id));

            foreach (var product in initialProducts)
            {
                ImageApiMock.SetGetMock(product.Image!.FileName, InitialImage);
            }
        }

        public async Task AssertCommonExpectations(int totalProductsCount, IEnumerable<long>? exceptIds = null)
        {
            //Expected products in db
            var dbProducts = await Context.Products.ToListAsync();

            if (exceptIds == null)
            {
                dbProducts.Should().BeEquivalentTo(initialProducts, o => o.Excluding(p => p.Image!.Url));
            }
            else
            {
                dbProducts
                    .Where(p => !exceptIds.Contains(p.Id))
                    .Should()
                    .BeEquivalentTo(
                        initialProducts.Where(p => !exceptIds.Contains(p.Id)),
                        o => o.Excluding(p => p.Image!.Url));
            }

            dbProducts.Count.Should().Be(totalProductsCount);
        }
    }
}