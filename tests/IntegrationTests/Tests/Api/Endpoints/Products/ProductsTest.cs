using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing.Builders;
using CrossCutting.Settings;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

        public async Task ValidateCommonExpectations(int totalProductsCount, IEnumerable<long>? exceptIds = null)
        {
            exceptIds ??= [];
            var productsToValidate = initialProducts.Where(p => !exceptIds.Contains(p.Id)).ToList();

            //Expected products in db
            var dbProducts = await Context.Products.ToListAsync();
            dbProducts.Where(p => !exceptIds.Contains(p.Id)).Should().BeEquivalentTo(productsToValidate, o => o.Excluding(p => p.ImageUrl));
            dbProducts.Count.Should().Be(totalProductsCount);

            //Expected image files
            var settings = WebApplicationFactoryFixture.Services.GetRequiredService<ITemplateSettings>();
            var imageFiles = Directory.GetFiles(settings.ImagesStoragePath);

            foreach (var product in productsToValidate)
            {
                var imageFile = imageFiles.SingleOrDefault(f => Path.GetFileNameWithoutExtension(f) == product.Id.ToString());
                imageFile.Should().NotBeNull($"image file for product {product.Id} should exist");

                var imageContent = File.ReadAllBytes(imageFile!);
                imageContent.Should().BeEquivalentTo(InitialImage, $"image content for product {product.Id} should match");
            }

            //Expected same image files and products count
            imageFiles.Length.Should().Be(totalProductsCount, "image count should match expected products count");
        }
    }
}