using Application.Services;
using Application.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Products;

internal static class UpdateProductEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:long}", static async (
            long id,
            [FromForm] UpdateProductRequest request,
            ProductService productService) =>
        {
            var product = await productService.Update(id, request).ConfigureAwait(false);
            return Results.Ok(product);
        });
    }
}