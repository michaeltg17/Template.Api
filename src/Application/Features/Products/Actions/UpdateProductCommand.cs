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
        var existing = await context.Products.FindAsync(id)
            ?? throw new NotFoundException<Product>(id);

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
        LogProductUpdated(product.Id);
        return product;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Product with id '{id}' updated successfully.")]
    public partial void LogProductUpdated(long id);
}