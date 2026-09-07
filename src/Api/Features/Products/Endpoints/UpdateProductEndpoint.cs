using Api.Exceptions;
using Api.Features.Products.Models.Requests;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace Api.Features.Products.Endpoints;

internal sealed partial class UpdateProductEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/{id:long}", static async (
            long id,
            [FromForm] UpdateProductRequest request,
            AppDbContext context,
            ProductService productService,
            ILogger<UpdateProductEndpoint> logger) =>
        {
            var existing = await context.Products.FindAsync(id) ?? throw new NotFoundException<Product>(id);

            var product = productService.GetValidatedProductOrThrow(request, existing);

            if (request.Image == null)
            {
                await productService.DeleteImage(product);
            }
            else
            {
                await productService.SetImage(product, request.Image);
            }

            await context.SaveChangesAsync();
            LogProductUpdated(logger, product.Id);
            return Results.Ok(product);
        })
        .DisableAntiforgery();
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Product with id '{id}' updated successfully.")]
    private static partial void LogProductUpdated(ILogger<UpdateProductEndpoint> logger, long id);
}