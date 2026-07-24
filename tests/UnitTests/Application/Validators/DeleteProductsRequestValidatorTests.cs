using System.Linq.Expressions;
using Application.Models.Requests;
using Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace UnitTests.Application.Validators;

public sealed class DeleteProductsRequestValidatorTests
{
    readonly DeleteProductsRequestValidator validator = new();

    public static TheoryData<DeleteProductsRequest, Expression<Func<DeleteProductsRequest, object>>, bool> GetTestCases()
    {
        return new TheoryData<DeleteProductsRequest, Expression<Func<DeleteProductsRequest, object>>, bool>
        {
            // Valid: single positive id
            { new DeleteProductsRequest([1L]), r => r.Ids, true },
            // Valid: multiple positive ids
            { new DeleteProductsRequest([1L, 2L, 3L]), r => r.Ids, true },
            // Invalid: empty array
            { new DeleteProductsRequest([]), r => r.Ids, false },
            // Invalid: zero
            { new DeleteProductsRequest([0L]), r => r.Ids, false },
            // Invalid: negative
            { new DeleteProductsRequest([-1L]), r => r.Ids, false },
            // Invalid: mixed positive and negative
            { new DeleteProductsRequest([1L, -2L]), r => r.Ids, false },
            // Invalid: mixed positive and zero
            { new DeleteProductsRequest([1L, 0L]), r => r.Ids, false },
        };
    }

    [Theory]
    [MemberData(nameof(GetTestCases))]
    public void Validate_ShouldHaveExpectedResult(
        DeleteProductsRequest request,
        Expression<Func<DeleteProductsRequest, object>> property,
        bool isValid)
    {
        //When
        var result = validator.TestValidate(request);

        //Then
        if (isValid) result.ShouldNotHaveAnyValidationErrors();
        else result.ShouldHaveValidationErrorFor(property).Only();
    }
}