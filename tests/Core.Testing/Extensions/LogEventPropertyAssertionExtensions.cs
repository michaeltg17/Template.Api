using Serilog.Sinks.InMemory.Assertions;

namespace Core.Testing.Extensions
{
    public static class LogEventPropertyAssertionExtensions
    {
        public static LogEventAssertion WithValue<T>(
            this LogEventPropertyValueAssertions assertion,
            IEnumerable<T> values)
        {
            return assertion.WithValue($"[{string.Join(", ", values)}]");
        }
    }
}