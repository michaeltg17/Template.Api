using Application.Exceptions;
using Application.Features.Images;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Products.Actions
{
    public class GetProductByIdQuery(AppDbContext context, ProductService productService)
    {
        public async Task<Product> Execute(long id)
        {
            var product = await context.Products.FindAsync(id).ConfigureAwait(false)
                ?? throw new NotFoundException<Product>(id);
            productService.SetImageUrl(product);
            return product;
        }
    }
}
