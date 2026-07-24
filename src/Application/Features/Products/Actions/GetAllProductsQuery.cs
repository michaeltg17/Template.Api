using Microsoft.EntityFrameworkCore;
using Domain.Models;
using Persistence;

namespace Application.Features.Products.Actions;

public class GetAllProductsQuery(AppDbContext context, ProductService productService)
{
    public async Task<IEnumerable<Product>> Execute()
    {
        var products = await context.Products
            .ToListAsync()
            .ConfigureAwait(false);

        foreach (var product in products)
            product.ImageUrl = productService.BuildImageUrl(product.Id);

        return products;
    }
}