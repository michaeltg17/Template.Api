using Application.Features.Products;
using Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Products;

internal static class DeleteProductsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/", static async (
            [FromBody] DeleteProductsRequest request,
            DeleteProductsCommand deleteProductsCommand) =>
        {
            var response = await deleteProductsCommand.Execute(request).ConfigureAwait(false);
            return Results.Ok(response);
        });
    }
}