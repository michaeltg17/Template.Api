using Api.Exceptions;
using Domain.Models;
using Persistence;

namespace Api.Features.Products.Endpoints;

internal static class GetProductEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:long}", static async (long id, AppDbContext context, ProductService productService) =>
        {
            var product = await context.Products.FindAsync(id) ?? throw new NotFoundException<Product>(id);
            productService.SetImageUrl(product);
            return Results.Ok(product);
        });
    }
}