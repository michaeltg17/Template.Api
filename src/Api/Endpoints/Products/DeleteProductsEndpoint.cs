using Application.Features.Products.Actions;
using Application.Features.Products.Models.Requests;
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
            var response = await deleteProductsCommand.Execute(request);
            return Results.Ok(response);
        });
    }
}