using FluentValidation;
using Application.Exceptions;
using Application.Models.Requests;
using Application.Models.Responses;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Microsoft.Extensions.Logging;
using CrossCutting.Logging;

namespace Application.Services
{
    public class ProductService(AppDbContext context, IValidator<Product> validator, ILogger<ProductService> logger)
    {
        public async Task<Product> GetById(long id)
        {
            var product = await context.Products.FindAsync(id).ConfigureAwait(false);
            if (product is null)
                throw new NotFoundException<Product>(id);

            return product;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await context.Products
                .ToListAsync()
                .ConfigureAwait(false);
        }

        public async Task<Product> Create(CreateProductRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
            };
            validator.ValidateAndThrow(product);
            await context.Products.AddAsync(product).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
            logger.LogProductCreated(product.Id);
            return product;
        }

        public async Task<Product> Update(long id, UpdateProductRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            var product = await context.Products.FindAsync(id).ConfigureAwait(false);
            if (product is null)
                throw new NotFoundException<Product>(id);

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            validator.ValidateAndThrow(product);
            await context.SaveChangesAsync().ConfigureAwait(false);
            logger.LogProductUpdated(product.Id);
            return product;
        }

        public async Task<DeleteProductsResponse> Delete(DeleteProductsRequest request)
        {
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
            }

            logger.LogProductsDeleted(foundIds);

            return new DeleteProductsResponse([.. foundIds], notFoundIds);
        }
    }
}