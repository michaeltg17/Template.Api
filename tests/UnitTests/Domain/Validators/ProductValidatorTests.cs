using FluentValidation.TestHelper;
using Domain.Models;
using Domain.Validators;
using Core.Testing.Builders;
using Xunit;

namespace UnitTests.Domain.Validators;

public sealed class ProductValidatorTests
{
    readonly ProductValidator validator = new();

    public static readonly TheoryDataRow<string, object?, bool>[] TestCases =
    [
        new(nameof(Product.Name), null, false) { TestDisplayName = "Name - Invalid: null" },
        new(nameof(Product.Name), "      ", false) { TestDisplayName = "Name - Invalid: whitespace" },
        new(nameof(Product.Name), "", false) { TestDisplayName = "Name - Invalid: empty" },
        new(nameof(Product.Name), new string('x', 201), false) { TestDisplayName = "Name - Invalid: exceeds 200" },
        new(nameof(Product.Name), new string('x', 200), true) { TestDisplayName = "Name - Valid: max 200" },

        new(nameof(Product.Description), null, false) { TestDisplayName = "Description - Invalid: null" },
        new(nameof(Product.Description), "      ", false) { TestDisplayName = "Description - Invalid: whitespace" },
        new(nameof(Product.Description), "", false) { TestDisplayName = "Description - Invalid: empty" },
        new(nameof(Product.Description), new string('x', 2001), false) { TestDisplayName = "Description - Invalid: exceeds 2000" },
        new(nameof(Product.Description), new string('x', 2000), true) { TestDisplayName = "Description - Valid: max 2000" },

        new(nameof(Product.Price), 0m, false) { TestDisplayName = "Price - Invalid: zero" },
        new(nameof(Product.Price), -5m, false) { TestDisplayName = "Price - Invalid: negative" },
        new(nameof(Product.Price), 10m, true) { TestDisplayName = "Price - Valid: positive" },
    ];

    [Theory]
    [MemberData(nameof(TestCases))]
    public void Cases(string propertyName, object? value, bool isValid)
    {
        var product = new ProductBuilder().WithValue(propertyName, value).Build();
        var result = validator.TestValidate(product);

        if (isValid) result.ShouldNotHaveAnyValidationErrors();
        else result.ShouldHaveValidationErrorFor(propertyName).Only();
    }
}