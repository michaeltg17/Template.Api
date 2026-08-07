using System.Text.Json;
using Domain.Models;
using UnitTests.Domain.Validators;
using Xunit.Sdk;

[assembly: RegisterXunitSerializer(typeof(ProductSerializer), typeof(Product), typeof(Image))]

namespace UnitTests.Domain.Validators;

public sealed class ProductSerializer : IXunitSerializer
{
    public bool IsSerializable(Type type, object? value, out string? failureReason)
    {
        failureReason = null;
        return type == typeof(Product) || type == typeof(Image);
    }

    public string Serialize(object value) => JsonSerializer.Serialize(value);

    public object Deserialize(Type type, string serializedValue) =>
        type == typeof(Product) ? JsonSerializer.Deserialize<Product>(serializedValue)! :
        type == typeof(Image) ? JsonSerializer.Deserialize<Image>(serializedValue)! :
        throw new ArgumentException($"Unsupported type: {type}", nameof(type));
}
