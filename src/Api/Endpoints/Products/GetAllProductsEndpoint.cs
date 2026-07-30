using Application.Features.Products.Actions;

namespace Api.Endpoints.Products;

internal static class GetAllProductsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", static async (GetAllProductsQuery getAllProductsQuery) =>
        {
            var products = await getAllProductsQuery.Execute();
            return Results.Ok(products);
        });
    }
}