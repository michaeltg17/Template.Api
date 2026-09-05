using Persistence;
using Microsoft.EntityFrameworkCore;

namespace Api.Features.Products.Endpoints;

internal static class GetAllProductsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/", static async (AppDbContext context, ProductService productService) =>
        {
            var products = await context.Products.ToListAsync();

            foreach (var product in products)
            {
                productService.SetImageUrl(product);
            }

            return Results.Ok(products);
        });
    }
}