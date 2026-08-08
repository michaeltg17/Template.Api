using System.Text.Json;
using IntegrationTests.Tests.Api.ApiBehaviourTests;

[assembly: Xunit.Sdk.RegisterXunitSerializerAttribute(typeof(IntegrationTests.BadRequestCaseSerializer), typeof(BadRequestTests.BadRequestCase))]

namespace IntegrationTests
{
    public class BadRequestCaseSerializer : Xunit.Sdk.IXunitSerializer
    {
        public bool IsSerializable(Type type, object? value, out string? reason)
        {
            reason = null;
            return type == typeof(BadRequestTests.BadRequestCase);
        }

        public string Serialize(object value)
        {
            var @case = (BadRequestTests.BadRequestCase)value;
            return JsonSerializer.Serialize(new
            {
                @case.Id,
                @case.Date,
                @case.Request,
                @case.ExpectedInstance,
                @case.ExpectedDetail
            });
        }

        public object Deserialize(Type type, string data)
        {
            var json = JsonSerializer.Deserialize<JsonElement>(data);
            return new BadRequestTests.BadRequestCase
            {
                Id = DeserializeObject(json.GetProperty("Id")),
                Date = DeserializeObject(json.GetProperty("Date")),
                Request = DeserializeObject(json.GetProperty("Request")),
                ExpectedInstance = json.GetProperty("ExpectedInstance").GetString()!,
                ExpectedDetail = json.GetProperty("ExpectedDetail").GetString()!,
            };
        }

        static object? DeserializeObject(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.String:
                    return element.GetString()!;
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var l))
                        return l;
                    return element.GetDouble();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                default:
                    return element.ToString();
            }
        }
    }
}
