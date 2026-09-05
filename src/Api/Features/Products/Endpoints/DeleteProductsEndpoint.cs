using Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Persistence;
using Microsoft.EntityFrameworkCore;
using Api.Features.Products.Models.Requests;
using Api.Features.Products.Models.Responses;
using Api.Exceptions;

namespace Api.Features.Products.Endpoints;

internal partial class DeleteProductsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapDelete("/", static async (
            [FromBody] DeleteProductsRequest request,
            AppDbContext context,
            ProductService productService,
            IValidator<DeleteProductsRequest> deleteRequestValidator,
            ILogger<DeleteProductsEndpoint> logger) =>
        {
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
                LogProductsDeleted(logger, foundIdsOrdered);
                return new DeleteProductsResponse(foundIdsOrdered, notFoundIds);
            }

            return new DeleteProductsResponse([], notFoundIds);
        });
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Products with ids '{ids}' deleted successfully.")]
    private static partial void LogProductsDeleted(ILogger<DeleteProductsEndpoint> logger, IEnumerable<long> ids);
}