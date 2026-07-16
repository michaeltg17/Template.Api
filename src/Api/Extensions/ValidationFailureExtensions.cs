using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace Api.Extensions
{
    public static class ValidationFailureExtensions
    {
        public static IDictionary<string, string[]> ToValidationProblemErrors(
            this IEnumerable<ValidationFailure> validationFailures)
        {
            return validationFailures
                .GroupBy(vf => vf.PropertyName)
                .ToDictionary(gvf => gvf.Key, gvf => gvf.Select(vf => vf.ErrorMessage).ToArray());
        }
    }
}