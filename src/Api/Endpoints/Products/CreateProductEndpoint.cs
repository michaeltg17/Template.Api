using Microsoft.AspNetCore.Mvc;
using Application.Features.Products;
using Application.Features.Products.Models.Requests;

namespace Api.Endpoints.Products;

internal static class CreateProductEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", static async (
            [FromForm] CreateProductRequest request,
            CreateProductCommand createProductCommand) =>
        {
            var product = await createProductCommand.Execute(request).ConfigureAwait(false);
            return Results.Created($"/api/Products/{product.Id}", product);
        });
    }
}