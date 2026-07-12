using ApiClient.Extensions;
using Application.Models.Requests;
using AwesomeAssertions;
using Core.Testing.Builders;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Products
{
    [Collection(nameof(ApiCollection))]
    public class CreateProductEndpointTests : Test
    {
        [Fact]
        public async Task CreateProductOk()
        {
            //When
            var request = new CreateProductRequest("New Product", "A description", 9.99m);
            var response = await ApiClient.CreateProduct(JsonContent.Create(request));
            var product = await response.To<Product>();

            //Then
            var expected = new ProductBuilder()
                .WithValues(p =>
                {
                    p.Id = product.Id;
                    p.Name = request.Name;
                    p.Description = request.Description;
                    p.Price = request.Price;
                })
                .Build();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            product.Id.Should().BeGreaterThan(0);
            product.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task ValidRequest_AutoAssignsId()
        {
            var request = new CreateProductRequest("Id Product", "Desc", 1m);
            var response = await ApiClient.CreateProduct(JsonContent.Create(request));
            var product = await response.To<Product>();

            product.Id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task ValidRequest_PersistedInDatabase()
        {
            var request = new CreateProductRequest("Persisted", "Persisted desc", 5.50m);
            var response = await ApiClient.CreateProduct(JsonContent.Create(request));
            var product = await response.To<Product>();

            var dbProduct = await Context.Products.FindAsync(product.Id);

            var expected = new ProductBuilder()
                .WithValues(p =>
                {
                    p.Id = product.Id;
                    p.Name = request.Name;
                    p.Description = request.Description;
                    p.Price = request.Price;
                })
                .Build();

            dbProduct.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task NoBody_ReturnsBadRequest()
        {
            var response = await ApiClient.CreateProduct(null);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}