using Microsoft.Extensions.Logging;
using Application.Exceptions;
using FluentValidation;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Features.Products.Models.Requests;
using Application.Features.Products.Models.Responses;
using Application.Features.Products.Logging;

namespace Application.Features.Products.Actions;

public class DeleteProductsCommand(
    AppDbContext context,
    ProductService productService,
    IValidator<DeleteProductsRequest> deleteRequestValidator,
    ILogger<DeleteProductsCommand> logger)
{
    public async Task<DeleteProductsResponse> Execute(DeleteProductsRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        await deleteRequestValidator.ValidateAndThrowAsync(request).ConfigureAwait(false);

        var products = await context.Products
            .Where(p => request.Ids.Contains(p.Id))
            .ToListAsync()
            .ConfigureAwait(false);

        var foundIds = products.Select(p => p.Id).ToHashSet();
        var notFoundIds = request.Ids.Except(foundIds).ToArray();

        if (!request.IgnoreNotFound && notFoundIds.Length > 0)
            throw new NotAllFoundException<Product>(notFoundIds);

        if (products.Count > 0)
        {
            context.Products.RemoveRange(products);
            await context.SaveChangesAsync().ConfigureAwait(false);
            foreach (var product in products)
            {
                productService.DeleteImage(product.Id);
            }
        }

        if (foundIds.Count > 0)
        {
            logger.LogProductsDeleted(foundIds);
        }

        return new DeleteProductsResponse([.. foundIds], notFoundIds);
    }
}