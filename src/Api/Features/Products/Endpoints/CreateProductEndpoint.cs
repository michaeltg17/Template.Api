using Api.Extensions;
using Api.Features.Products.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace Api.Features.Products.Endpoints;

internal partial class CreateProductEndpoint
{
    public void Map(IEndpointRouteBuilder app)
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

            if (request.Image != null)
            {
                await productService.SetImage(product, request.Image);
                await context.SaveChangesAsync();
            }

            LogProductCreated(product.Id);

            return Results.Created($"{EndpointExtensions.ProductsPath}/{product.Id}", product);
        })
        .DisableAntiforgery();
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Product with id '{id}' created successfully.")]
    public partial void LogProductCreated(long id);
}