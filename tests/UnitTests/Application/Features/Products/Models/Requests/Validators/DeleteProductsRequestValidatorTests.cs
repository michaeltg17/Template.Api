using Application.Features.Products.Models.Requests;
using Application.Features.Products.Models.Requests.Validators;
using FluentValidation.TestHelper;
using Core.Testing.Builders;
using Xunit;

namespace UnitTests.Application.Features.Products.Models.Requests.Validators;

public sealed class DeleteProductsRequestValidatorTests
{
    readonly DeleteProductsRequestValidator validator = new();

    public static readonly TheoryDataRow<string, object?, bool>[] TestCases =
    [
        new(nameof(DeleteProductsRequest.Ids), new long[] { 1L }, true) { TestDisplayName = "Ids - Valid: single positive" },
        new(nameof(DeleteProductsRequest.Ids), new long[] { 1L, 2L, 3L }, true) { TestDisplayName = "Ids - Valid: multiple positive" },
        new(nameof(DeleteProductsRequest.Ids), Array.Empty<long>(), false) { TestDisplayName = "Ids - Invalid: empty" },
        new(nameof(DeleteProductsRequest.Ids), null, false) { TestDisplayName = "Ids - Invalid: null" },
        new(nameof(DeleteProductsRequest.Ids), new long[] { 0L }, false) { TestDisplayName = "Ids - Invalid: zero" },
        new(nameof(DeleteProductsRequest.Ids), new long[] { -1L }, false) { TestDisplayName = "Ids - Invalid: negative" },
        new(nameof(DeleteProductsRequest.Ids), new long[] { 1L, -2L }, false) { TestDisplayName = "Ids - Invalid: mixed positive and negative" },
        new(nameof(DeleteProductsRequest.Ids), new long[] { 1L, 0L }, false) { TestDisplayName = "Ids - Invalid: mixed positive and zero" },
    ];

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Cases(string propertyName, object? value, bool isValid)
    {
        //When
        var request = new DeleteProductsRequestBuilder().WithValue(propertyName, value).Build();
        var result = validator.TestValidate(request);

        //Then
        if (isValid) result.ShouldNotHaveAnyValidationErrors();
        else result.ShouldHaveValidationErrorFor(propertyName).Only();
    }
}
