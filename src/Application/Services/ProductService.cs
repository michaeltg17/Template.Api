using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Application.Models.Requests;
using Application.Models.Responses;
using CrossCutting.Logging;
using CrossCutting.Settings;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Domain.Models;
using Flurl;

namespace Application.Services
{
    public class ProductService(
        AppDbContext context,
        IValidator<Product> productValidator,
        IValidator<CreateProductRequest> requestValidator,
        IValidator<DeleteProductsRequest> deleteRequestValidator,
        ILogger<ProductService> logger,
        ITemplateSettings templateSettings)
    {
        public async Task<Product> GetById(long id)
        {
            var product = await context.Products.FindAsync(id).ConfigureAwait(false)
                ?? throw new NotFoundException<Product>(id);
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
            var product = GetValidatedProductOrThrow(request);

            await context.Products.AddAsync(product).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);

            if (request.Image != null)
            {
                await SaveImage(product.Id, request.Image)
                    .ConfigureAwait(false);
            }

            product.ImageUrl = BuildImageUrl(product.Id);
            logger.LogProductCreated(product.Id);
            return product;
        }

        public async Task<Product> Update(long id, UpdateProductRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);
            var existing = await context.Products.FindAsync(id).ConfigureAwait(false)
                ?? throw new NotFoundException<Product>(id);

            var product = GetValidatedProductOrThrow(request, existing);

            if (request.Image != null)
            {
                DeleteImage(product.Id);
                await SaveImage(id, request.Image).ConfigureAwait(false);
            }

            await context.SaveChangesAsync().ConfigureAwait(false);
            product.ImageUrl = BuildImageUrl(product.Id);
            logger.LogProductUpdated(product.Id);
            return product;
        }

        public async Task<DeleteProductsResponse> Delete(DeleteProductsRequest request)
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
                    DeleteImage(product.Id);
                }
            }

            if (foundIds.Count > 0)
            {
                logger.LogProductsDeleted(foundIds);
            }

            return new DeleteProductsResponse([.. foundIds], notFoundIds);
        }

        async Task<string> SaveImage(long productId, IFormFile image)
        {
            var extension = Path.GetExtension(image.FileName);
            var safeFileName = $"{productId}{extension}";
            var fullPath = Path.Combine(templateSettings.ImagesStoragePath, safeFileName);

            using var stream = File.Create(fullPath);
            await image.CopyToAsync(stream).ConfigureAwait(false);

            return safeFileName;
        }

        string? FindImageFile(long productId, bool throwIfNotFound = true)
        {
            var foundFile =
                Directory.EnumerateFiles(templateSettings.ImagesStoragePath, $"{productId}.*")
                .SingleOrDefault(f => templateSettings.AllowedImageExtensions
                    .Contains(Path.GetExtension(f), StringComparer.InvariantCultureIgnoreCase));

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

        Product GetValidatedProductOrThrow(CreateProductRequest request, Product? existing = null)
        {
            var product = existing ?? new Product();
            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;

            var requestResult = requestValidator.Validate(request);
            var productResult = productValidator.Validate(product);

            var failures = new List<ValidationFailure>();
            if (!requestResult.IsValid) failures.AddRange(requestResult.Errors);
            if (!productResult.IsValid) failures.AddRange(productResult.Errors);
            return failures.Count > 0 ? throw new ValidationException(failures) : product;
        }
    }
}