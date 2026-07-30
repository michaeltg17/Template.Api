using Microsoft.AspNetCore.Mvc;
using Application.Features.Products.Models.Requests;
using Application.Features.Products.Actions;
using Api.Extensions;

namespace Api.Endpoints.Products;

internal static class CreateProductEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", static async (
            [FromForm] CreateProductRequest request,
            CreateProductCommand createProductCommand) =>
        {
            var product = await createProductCommand.Execute(request);
            return Results.Created($"{EndpointExtensions.ProductsPath}/{product.Id}", product);
        })
        .DisableAntiforgery();
    }
}