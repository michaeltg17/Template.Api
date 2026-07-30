using Application.Features.Products.Models.Requests;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Persistence;

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
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        if (request.Image != null)
        {
            await productService.SetImage(product, request.Image);
            await context.SaveChangesAsync();
        }

        LogProductCreated(product.Id);
        return product;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Product with id '{id}' created successfully.")]
    public partial void LogProductCreated(long id);
}