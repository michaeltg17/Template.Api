using Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Products;

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
            var product = await updateProductCommand.Execute(id, request).ConfigureAwait(false);
            return Results.Ok(product);
        });
    }
}