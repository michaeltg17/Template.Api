using Application.Services;
using Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Products;

internal static class CreateProductEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", static async (
            [FromForm] CreateProductRequest request,
            [FromServices] ProductService productService) =>
        {
            var product = await productService.Create(request).ConfigureAwait(false);
            return Results.Created($"/api/Products/{product.Id}", product);
        });
    }
}