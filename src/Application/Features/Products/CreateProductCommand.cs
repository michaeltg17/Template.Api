using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Application.Models.Requests;
using CrossCutting.Logging;
using Domain.Models;
using Persistence;

namespace Application.Features.Products;

public class CreateProductCommand(
    AppDbContext context,
    ProductService productService,
    ILogger<CreateProductCommand> logger)
{
    public async Task<Product> Execute(CreateProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var product = productService.GetValidatedProductOrThrow(request);

        await context.Products.AddAsync(product).ConfigureAwait(false);
        await context.SaveChangesAsync().ConfigureAwait(false);

        if (request.Image != null)
        {
            await productService.SaveImage(product.Id, request.Image).ConfigureAwait(false);
        }

        product.ImageUrl = productService.BuildImageUrl(product.Id);
        logger.LogProductCreated(product.Id);
        return product;
    }
}