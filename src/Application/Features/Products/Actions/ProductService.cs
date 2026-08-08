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
        public static string BuildImageFileName(Product product, string extension)
        {
            ArgumentNullException.ThrowIfNull(product);
            return $"{product.Id}{extension}";
        }

        internal async Task SetImage(Product product, IFormFile image)
        {
            var extension = Path.GetExtension(image.FileName);
            var fileName = BuildImageFileName(product, extension);

            using var stream = image.OpenReadStream();
            await imageService.Upload(fileName, stream, image.ContentType);

            product.Image = new Image(fileName, imageService.BuildUrl(fileName));
        }

        internal void SetImageUrl(Product product)
        {
            if (product.Image is not null)
                product.Image = new Image(product.Image.FileName, imageService.BuildUrl(product.Image.FileName));
        }

        internal async Task DeleteImage(Product product)
        {
            if (product.Image!.FileName is not null)
                await imageService.Delete(product.Image!.FileName);
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