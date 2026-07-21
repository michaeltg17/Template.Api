using Application.Exceptions;
using Application.Models.Requests;
using Application.Models.Responses;
using CrossCutting.Logging;
using CrossCutting.Settings;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Domain.Models;
using Flurl;

namespace Application.Services
{
    public class ProductService(
        AppDbContext context,
        IValidator<Product> validator,
        ILogger<ProductService> logger,
        ITemplateSettings templateSettings)
    {
        public async Task<Product> GetById(long id)
        {
            var product = await context.Products.FindAsync(id).ConfigureAwait(false);
            if (product is null)
                throw new NotFoundException<Product>(id);

            product.ImageUrl = BuildImageUrl(product.Id);
            return product;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            var products = await context.Products
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var product in products)
                product.ImageUrl = BuildImageUrl(product.Id);

            return products;
        }

        public async Task<Product> Create(CreateProductRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price
            };
            validator.ValidateAndThrow(product);

            await context.Products.AddAsync(product).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            if (request.ImageFileName != null)
            {
                await SaveImage(product.Id, request.ImageData!, request.ImageFileName)
                    .ConfigureAwait(false);
            }

            product.ImageUrl = BuildImageUrl(product.Id);
            logger.LogProductCreated(product.Id);
            return product;
        }

        public async Task<Product> Update(long id, UpdateProductRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            var product = await context.Products.FindAsync(id).ConfigureAwait(false)
                ?? throw new NotFoundException<Product>(id);

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            validator.ValidateAndThrow(product);

            if (request.ImageFileName != null)
            {
                DeleteImage(product.Id);
                await SaveImage(id, request.ImageData!, request.ImageFileName).ConfigureAwait(false);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
            product.ImageUrl = BuildImageUrl(product.Id);
            logger.LogProductUpdated(product.Id);
            return product;
        }

        public async Task<DeleteProductsResponse> Delete(DeleteProductsRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
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
                    DeleteImage(product.Id);
                }
            }

            logger.LogProductsDeleted(foundIds);

            return new DeleteProductsResponse([.. foundIds], notFoundIds);
        }

        async Task<string> SaveImage(long productId, byte[] imageData, string? fileName)
        {
            if (imageData.Length > templateSettings.MaxImageSizeMb * 1024L * 1024)
                throw new TemplateException($"Image size exceeds the {templateSettings.MaxImageSizeMb}MB limit.");

            var extension = fileName != null ? Path.GetExtension(fileName).ToLowerInvariant() : ".png";
            if (!templateSettings.AllowedImageExtensions.Contains(extension))
                throw new TemplateException($"Invalid image format. Allowed formats are: {string.Join(", ", templateSettings.AllowedImageExtensions.Select(e => e.TrimStart('.')))}");

            Directory.CreateDirectory(templateSettings.ImagesStoragePath);

            var safeFileName = $"{productId}{extension}";
            var fullPath = Path.Combine(templateSettings.ImagesStoragePath, safeFileName);

            await File.WriteAllBytesAsync(fullPath, imageData).ConfigureAwait(false);

            return safeFileName;
        }

        string? FindImageFile(long productId, bool throwIfNotFound = true)
        {
            var foundFile = Directory.EnumerateFiles(
                templateSettings.ImagesStoragePath,
                $"{productId}.*")
                .SingleOrDefault(f => templateSettings.AllowedImageExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));

            return throwIfNotFound && foundFile is null
                ? throw new TemplateException($"Expected product with id '{productId}' to have an image to be deleted.")
                : foundFile;
        }

        void DeleteImage(long productId)
        {
            var fullPath = FindImageFile(productId, throwIfNotFound: true)!;
            File.Delete(fullPath);
        }

        string? BuildImageUrl(long productId)
        {
            var foundFile = FindImageFile(productId, throwIfNotFound: false);
            if (foundFile is null)
                return null;

            var fileName = Path.GetFileName(foundFile);
            return Url.Combine(templateSettings.ApiUrl, templateSettings.ImagesRequestPath, fileName);
        }
    }
}