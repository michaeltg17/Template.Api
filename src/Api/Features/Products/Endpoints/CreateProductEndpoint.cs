using Api.Extensions;
using Api.Features.Products.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace Api.Features.Products.Endpoints;

internal sealed partial class CreateProductEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/", static async (
            [FromForm] CreateProductRequest request,
            AppDbContext context,
            ProductService productService,
            ILogger<CreateProductEndpoint> logger) =>
        {
            var product = productService.GetValidatedProductOrThrow(request);

            await context.Products.AddAsync(product);
            await context.SaveChangesAsync();

            if (request.Image is not null)
            {
                await productService.SetImage(product, request.Image);
                await context.SaveChangesAsync();
            }

            LogProductCreated(logger, product.Id);

            return Results.Created(
                $"{EndpointExtensions.ProductsPath}/{product.Id}",
                product);
        })
        .DisableAntiforgery();
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Product with id '{id}' created successfully.")]
    private static partial void LogProductCreated(ILogger<CreateProductEndpoint> logger, long id);
}
