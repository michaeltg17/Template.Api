using System.Reflection;

namespace IntegrationTests.Extensions
{
    public static class ObjectExtensions
    {
        public static Dictionary<string, string?> ToDictionary(this object @object)
        {
            ArgumentNullException.ThrowIfNull(@object);

            return @object.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(p => p.Name, p => p.GetValue(@object)?.ToString());
        }
    }
}
