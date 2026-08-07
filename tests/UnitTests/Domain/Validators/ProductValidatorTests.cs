using FluentValidation.TestHelper;
using Domain.Models;
using Domain.Validators;
using Core.Testing.Builders;
using Xunit;

namespace UnitTests.Domain.Validators;

public sealed class ProductValidatorTests
{
    readonly ProductValidator validator = new();

    public static readonly TheoryDataRow<Product, string, bool>[] BuildCases =
    [
        new(new ProductBuilder().WithValues(p => p.Name = null!).Build(), nameof(Product.Name), false)
        {
            TestDisplayName = "Name - Invalid: null"
        },
        new(new ProductBuilder().WithValues(p => p.Name = "      ").Build(), nameof(Product.Name), false)
        {
            TestDisplayName = "Name - Invalid: whitespace"
        },
        new(new ProductBuilder().WithValues(p => p.Name = "").Build(), nameof(Product.Name), false)
        {
            TestDisplayName = "Name - Invalid: empty"
        },
        new(new ProductBuilder().WithValues(p => p.Name = new string('x', 201)).Build(), nameof(Product.Name), false)
        {
            TestDisplayName = "Name - Invalid: exceeds 200"
        },
        new(new ProductBuilder().WithValues(p => p.Name = new string('x', 200)).Build(), nameof(Product.Name), true)
        {
            TestDisplayName = "Name - Valid: max 200"
        },

        new(new ProductBuilder().WithValues(p => p.Description = null!).Build(), nameof(Product.Description), false)
        {
            TestDisplayName = "Description - Invalid: null"
        },
        new(new ProductBuilder().WithValues(p => p.Description = "      ").Build(), nameof(Product.Description), false)
        {
            TestDisplayName = "Description - Invalid: whitespace"
        },
        new(new ProductBuilder().WithValues(p => p.Description = "").Build(), nameof(Product.Description), false)
        {
            TestDisplayName = "Description - Invalid: empty"
        },
        new(new ProductBuilder().WithValues(p => p.Description = new string('x', 2001)).Build(), nameof(Product.Description), false)
        {
            TestDisplayName = "Description - Invalid: exceeds 2000"
        },
        new(new ProductBuilder().WithValues(p => p.Description = new string('x', 2000)).Build(), nameof(Product.Description), true)
        {
            TestDisplayName = "Description - Valid: max 2000"
        },

        new(new ProductBuilder().WithValues(p => p.Price = 0m).Build(), nameof(Product.Price), false)
        {
            TestDisplayName = "Price - Invalid: zero"
        },
        new(new ProductBuilder().WithValues(p => p.Price = -5m).Build(), nameof(Product.Price), false)
        {
            TestDisplayName = "Price - Invalid: negative"
        },
        new(new ProductBuilder().WithValues(p => p.Price = 10m).Build(), nameof(Product.Price), true)
        {
            TestDisplayName = "Price - Valid: positive"
        },
    ];

    [Theory]
    [MemberData(nameof(BuildCases))]
    public void Cases(Product product, string propertyName, bool isValid)
    {
        //When
        var result = validator.TestValidate(product);

        //Then
        if (isValid) result.ShouldNotHaveAnyValidationErrors();
        else result.ShouldHaveValidationErrorFor(propertyName).Only();
    }
}