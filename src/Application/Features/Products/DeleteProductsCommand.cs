using Microsoft.Extensions.Logging;
using Application.Models.Requests;
using Application.Models.Responses;
using Application.Exceptions;
using CrossCutting.Logging;
using FluentValidation;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Features.Products;

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