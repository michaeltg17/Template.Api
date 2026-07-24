using Application.Models.Requests;
using CrossCutting.Settings;
using FluentValidation;

namespace Application.Validators;

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator(ITemplateSettings templateSettings)
    {
        RuleFor(x => x.Image)
            .Must((request, image) => image == null || image.Length > 0)
            .WithMessage("Image can be null but if not, length cannot be 0.");

        RuleFor(x => x.Image)
            .Must((request, image) => image == null || image.Length <= templateSettings.MaxImageSizeMb * 1024L * 1024)
            .WithMessage($"Image size exceeds the '{templateSettings.MaxImageSizeMb}' MB limit.");

        RuleFor(x => x.Image)
            .Custom((image, ctx) =>
            {
                if (image == null) return;
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                if (templateSettings.AllowedImageExtensions.Contains(extension)) return;

                var extensions = string.Join(", ", templateSettings.AllowedImageExtensions.Select(e => e.TrimStart('.')));
                ctx.AddFailure($"Invalid image with extension '{extension}'. Allowed extensions are: {extensions}");
            });
    }
}