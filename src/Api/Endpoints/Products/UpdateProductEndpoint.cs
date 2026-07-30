using Microsoft.AspNetCore.Mvc;
using Application.Features.Products.Models.Requests;
using Application.Features.Products.Actions;

namespace Api.Endpoints.Products;

internal static class UpdateProductEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:long}", static async (
            long id,
            [FromForm] UpdateProductRequest request,
            UpdateProductCommand updateProductCommand) =>
        {
            var product = await updateProductCommand.Execute(id, request);
            return Results.Ok(product);
        })
        .DisableAntiforgery();
    }
}