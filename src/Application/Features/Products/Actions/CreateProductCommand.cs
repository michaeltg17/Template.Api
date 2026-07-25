using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Domain.Models;
using Persistence;
using Application.Features.Products.Models.Requests;

namespace Application.Features.Products.Actions;

public partial class CreateProductCommand(
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
            var imageName = await productService.SaveImage(product.Id, request.Image)
                .ConfigureAwait(false);
            product.ImageName = imageName;
            product.ImageUrl = productService.BuildImageUrl(imageName);
        }

        LogProductCreated(product.Id);
        return product;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Product with id '{id}' created successfully.")]
    public partial void LogProductCreated(long id);
}