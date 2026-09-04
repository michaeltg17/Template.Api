using FluentValidation;

namespace Api.Features.Products.Models.Requests.Validators;

public sealed class DeleteProductsRequestValidator : AbstractValidator<DeleteProductsRequest>
{
    public DeleteProductsRequestValidator()
    {
        RuleFor(x => x.Ids)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(ids => ids.All(id => id > 0))
            .WithMessage("All ids must be positive.");
    }
}