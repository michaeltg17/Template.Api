using Application.Exceptions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Products
{
    public class GetProductByIdQuery(AppDbContext context, ProductService productService)
    {
        public async Task<Product> Execute(long id)
        {
            var product = await context.Products.FindAsync(id).ConfigureAwait(false)
                ?? throw new NotFoundException<Product>(id);
            product.ImageUrl = productService.BuildImageUrl(product.Id);
            return product;
        }
    }
}
