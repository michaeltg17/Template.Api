using Application.Exceptions;
using Domain.Models;
using Persistence;

namespace Application.Features.Products.Actions
{
    public class GetProductByIdQuery(AppDbContext context, ProductService productService)
    {
        public async Task<Product> Execute(long id)
        {
            var product = await context.Products.FindAsync(id)
                ?? throw new NotFoundException<Product>(id);
            productService.SetImageUrl(product);
            return product;
        }
    }
}
