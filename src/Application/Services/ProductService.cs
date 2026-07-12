using Application.Exceptions;
using Application.Models.Requests;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Services
{
    public class ProductService(AppDbContext context)
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
                .OrderBy(p => p.Id)
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
            context.Products.Add(product);
            await context.SaveChangesAsync().ConfigureAwait(false);
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
            await context.SaveChangesAsync().ConfigureAwait(false);
            return product;
        }

        public async Task Delete(long id)
        {
            var product = await context.Products.FindAsync(id).ConfigureAwait(false);
            if (product is null)
                throw new NotFoundException<Product>(id);

            context.Products.Remove(product);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}