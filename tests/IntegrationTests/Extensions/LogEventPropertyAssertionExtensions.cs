using DragoAnt.Assertions.Serilog;

namespace IntegrationTests.Extensions
{
    public static class LogEventPropertyAssertionExtensions
    {
        public static LogEventAssertion WithValues<T>(
            this LogEventPropertyValueAssertions assertion,
            IEnumerable<T> values)
        {
            return assertion.WithValue($"[{string.Join(", ", values)}]");
        }
    }
}