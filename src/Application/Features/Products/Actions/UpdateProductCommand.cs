using Application.Exceptions;
using Application.Features.Products.Models.Requests;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Persistence;

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

        if (request.Image == null)
        {
            await productService.DeleteImage(product).ConfigureAwait(false);
        }
        else
        {
            await productService.SetImage(product, request.Image).ConfigureAwait(false);
        }

        await context.SaveChangesAsync().ConfigureAwait(false);
        LogProductUpdated(product.Id);
        return product;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Product with id '{id}' updated successfully.")]
    public partial void LogProductUpdated(long id);
}