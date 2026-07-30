using Microsoft.Extensions.Logging;
using Application.Exceptions;
using FluentValidation;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Features.Products.Models.Requests;
using Application.Features.Products.Models.Responses;

namespace Application.Features.Products.Actions;

public partial class DeleteProductsCommand(
    AppDbContext context,
    ProductService productService,
    IValidator<DeleteProductsRequest> deleteRequestValidator,
    ILogger<DeleteProductsCommand> logger)
{
    public async Task<DeleteProductsResponse> Execute(DeleteProductsRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        await deleteRequestValidator.ValidateAndThrowAsync(request);

        var products = await context.Products
            .Where(p => request.Ids.Contains(p.Id))
            .ToListAsync();

        var foundIds = products.Select(p => p.Id).ToHashSet();
        var notFoundIds = request.Ids.Except(foundIds).ToArray();

        if (!request.IgnoreNotFound && notFoundIds.Length > 0)
            throw new NotAllFoundException<Product>(notFoundIds);

        if (products.Count > 0)
        {
            context.Products.RemoveRange(products);
            await context.SaveChangesAsync();
            foreach (var product in products)
            {
                await productService.DeleteImage(product);
            }
        }

        if (foundIds.Count > 0)
        {
            var foundIdsOrdered = request.Ids.Where(foundIds.Contains).ToArray();
            LogProductsDeleted(foundIdsOrdered);
            return new DeleteProductsResponse(foundIdsOrdered, notFoundIds);
        }

        return new DeleteProductsResponse([], notFoundIds);
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Products with ids '{ids}' deleted successfully.")]
    public partial void LogProductsDeleted(IEnumerable<long> ids);
}