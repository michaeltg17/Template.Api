using ApiClient.Extensions;
using AwesomeAssertions;
using Domain.Models;
using System.Net;
using Xunit;

namespace FunctionalTests.Tests.Products
{
    public class GetTest : Test
    {
        [Fact]
        public async Task GetAllProducts_ThenGetByIdIfAny()
        {
            //Get all products ok
            var getAllProductsResponse = await ApiClient.GetAllProducts();
            var products = await getAllProductsResponse.To<List<Product>>();
            getAllProductsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            if (products is not { Count: > 0 })
            {
                products.Should().BeEmpty();
                return;
            }

            //Get by id ok
            var product = products[0];
            var getByIdResponse = await ApiClient.GetProduct(product.Id);
            var getByIdProduct = await getByIdResponse.To<Product>();
            getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getByIdProduct.Should().BeEquivalentTo(product);
        }
    }
}