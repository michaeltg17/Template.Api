using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ApiClient.Validators
{
    public sealed class ProblemDetailsValidator : AbstractValidator<ProblemDetails>
    {
        public ProblemDetailsValidator()
        {
            RuleFor(p => p.Type)
                .NotNull().WithMessage("Type is null.")
                .NotEmpty().WithMessage("Type is empty or whitespace.");

            RuleFor(p => p.Title)
                .NotNull().WithMessage("Title is null.")
                .NotEmpty().WithMessage("Title is empty or whitespace.");

            RuleFor(p => p.Status)
                .NotNull().WithMessage("Status is null.");
        }
    }
}