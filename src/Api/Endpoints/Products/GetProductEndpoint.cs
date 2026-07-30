using Application.Features.Products.Actions;

namespace Api.Endpoints.Products;

internal static class GetProductEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:long}", static async (long id, GetProductByIdQuery getProductByIdQuery) =>
        {
            var product = await getProductByIdQuery.Execute(id);
            return Results.Ok(product);
        });
    }
}