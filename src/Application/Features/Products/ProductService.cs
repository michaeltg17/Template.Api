using Microsoft.AspNetCore.Http;
using Application.Exceptions;
using CrossCutting.Settings;
using FluentValidation;
using FluentValidation.Results;
using Domain.Models;
using Flurl;
using Application.Features.Products.Models.Requests;

namespace Application.Features.Products
{
    public class ProductService(
        IValidator<Product> productValidator,
        IValidator<CreateProductRequest> requestValidator,
        ITemplateSettings templateSettings)
    {
        internal async Task<string> SaveImage(long productId, IFormFile image)
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

        internal void DeleteImage(long productId)
        {
            var fullPath = FindImageFile(productId, throwIfNotFound: true)!;
            File.Delete(fullPath);
        }

        internal string? BuildImageUrl(long productId)
        {
            var foundFile = FindImageFile(productId, throwIfNotFound: false);
            if (foundFile is null)
                return null;

            var fileName = Path.GetFileName(foundFile);
            return Url.Combine(templateSettings.ApiUrl, templateSettings.ImagesRequestPath, fileName);
        }

        internal Product GetValidatedProductOrThrow(CreateProductRequest request, Product? existing = null)
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