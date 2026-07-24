using Application.Models.Requests;
using FluentValidation;

namespace Application.Validators;

public sealed class DeleteProductsRequestValidator : AbstractValidator<DeleteProductsRequest>
{
    public DeleteProductsRequestValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty()
            .Must(ids => ids.All(id => id > 0))
            .WithMessage("All ids must be positive.");
    }
}