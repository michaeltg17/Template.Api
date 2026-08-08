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
        new(nameof(Product.Name), null, false) { TestDisplayName = "Invalid: Name null" },
        new(nameof(Product.Name), "      ", false) { TestDisplayName = "Invalid: Name whitespace" },
        new(nameof(Product.Name), "", false) { TestDisplayName = "Invalid: Name empty" },
        new(nameof(Product.Name), new string('x', 201), false) { TestDisplayName = "Invalid: Name exceeds 200" },
        new(nameof(Product.Name), new string('x', 200), true) { TestDisplayName = "Valid: Name max 200" },

        new(nameof(Product.Description), null, false) { TestDisplayName = "Invalid: Description null" },
        new(nameof(Product.Description), "      ", false) { TestDisplayName = "Invalid: Description whitespace" },
        new(nameof(Product.Description), "", false) { TestDisplayName = "Invalid: Description empty" },
        new(nameof(Product.Description), new string('x', 2001), false) { TestDisplayName = "Invalid: Description exceeds 2000" },
        new(nameof(Product.Description), new string('x', 2000), true) { TestDisplayName = "Valid: Description max 2000" },

        new(nameof(Product.Price), 0m, false) { TestDisplayName = "Invalid: Price zero" },
        new(nameof(Product.Price), -5m, false) { TestDisplayName = "Invalid: Price negative" },
        new(nameof(Product.Price), 10m, true) { TestDisplayName = "Valid: Price positive" },
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