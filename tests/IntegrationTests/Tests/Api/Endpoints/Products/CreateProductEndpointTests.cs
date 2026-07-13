using ApiClient.Extensions;
using Application.Models.Requests;
using AwesomeAssertions;
using Core.Testing.Builders;
using Core.Testing.Validators;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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
            var response = await ApiClient.CreateProduct(request);
            var product = await response.To<Product>();

            //Then: retuns expected product
            var expected = new ProductBuilder()
                .WithValues(p =>
                {
                    p.Id = product.Id;
                    p.Name = request.Name;
                    p.Description = request.Description;
                    p.Price = request.Price;
                })
                .Build();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            product.Id.Should().BeGreaterThan(0);
            product.Should().BeEquivalentTo(expected);

            //Then: expected product in db
            var dbProduct = await Context.Products.FindAsync(product.Id);
            dbProduct.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task InvalidRequest_ExpectedProblemDetails()
        {
            var response = await ApiClient.CreateProduct(null);

            var problemDetails = await response.To<ProblemDetails>();
            var traceId = ProblemDetailsValidator.ValidateTraceId(problemDetails);

            var expected = new ProblemDetailsBuilder()
                .WithTraceId(traceId)
                .WithBadHttpRequestException()
                .WithInstance("/api/Products")
                .WithDetail("Required parameter \"CreateProductRequest createProductRequest\" was not provided from body.")
                .Build();

            problemDetails.Should().BeEquivalentTo(expected);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}