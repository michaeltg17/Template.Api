using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Features.Products.Actions;

public class GetAllProductsQuery(AppDbContext context, ProductService productService)
{
    public async Task<IEnumerable<Product>> Execute()
    {
        var products = await context.Products
            .ToListAsync();

        foreach (var product in products)
        {
            productService.SetImageUrl(product);
        }

        return products;
    }
}