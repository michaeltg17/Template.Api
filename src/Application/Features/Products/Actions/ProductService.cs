using Microsoft.AspNetCore.Http;
using FluentValidation;
using FluentValidation.Results;
using Domain.Models;
using Application.Features.Images;
using Application.Features.Products.Models.Requests;

namespace Application.Features.Products.Actions
{
    public class ProductService(
        IValidator<Product> productValidator,
        IValidator<CreateProductRequest> requestValidator,
        ImageService imageService)
    {
        internal async Task<string?> SaveImage(long productId, IFormFile image)
        {
            var extension = Path.GetExtension(image.FileName);
            var imageFileName = $"{productId}{extension}";

            using var stream = image.OpenReadStream();
            await imageService.UploadAsync(imageFileName, stream, image.ContentType)
                .ConfigureAwait(false);

            return imageFileName;
        }

        internal async Task DeleteImage(string? imageName)
        {
            if (imageName is not null)
                await imageService.DeleteAsync(imageName).ConfigureAwait(false);
        }

        internal string? BuildImageUrl(string? imageName)
        {
            return imageName is not null ? imageService.BuildUrl(imageName) : null;
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