using Application.Features.Products.Actions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Products;

internal static class GetAllProductsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", static async (GetAllProductsQuery getAllProductsQuery) =>
        {
            var products = await getAllProductsQuery.Execute().ConfigureAwait(false);
            return Results.Ok(products);
        });
    }
}