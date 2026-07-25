using Microsoft.Extensions.Logging;
using Application.Exceptions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Features.Products.Models.Requests;

namespace Application.Features.Products.Actions;

public partial class UpdateProductCommand(
    AppDbContext context,
    ProductService productService,
    ILogger<UpdateProductCommand> logger)
{
    public async Task<Product> Execute(long id, UpdateProductRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var existing = await context.Products.FindAsync(id).ConfigureAwait(false)
            ?? throw new NotFoundException<Product>(id);

        var product = productService.GetValidatedProductOrThrow(request, existing);

        if (request.Image != null)
        {
            string? oldImageName = existing.ImageName;

            var newImageName = await productService.SaveImage(product.Id, request.Image)
                .ConfigureAwait(false);

            if (oldImageName != newImageName)
            {
                await productService.DeleteImage(oldImageName).ConfigureAwait(false);
            }

            product.ImageName = newImageName;
            product.ImageUrl = productService.BuildImageUrl(newImageName);
        }

        await context.SaveChangesAsync().ConfigureAwait(false);
        LogProductUpdated(product.Id);
        return product;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Product with id '{id}' updated successfully.")]
    public partial void LogProductUpdated(long id);
}