using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IntegrationTests.Infrastructure
{
    [SuppressMessage("Performance", "CA1801:Unused parameters", Justification = "ILoggerFactory implementation")]
    sealed class DiagnosticMessagesLoggerFactory : ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider)
        {
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null!;

        public ILogger CreateLogger(string categoryName) => new DiagnosticLogger();

        public void Dispose()
        {
        }

        [SuppressMessage("Performance", "CA1801:Unused parameters", Justification = "ILogger implementation")]
        private sealed class DiagnosticLogger : ILogger
        {
            public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel)) return;
                var time = DateTime.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                var message = formatter(state, exception);
                if (exception is not null)
                {
                    message += "\n" + exception;
                }
                TestContext.Current.SendDiagnosticMessage($"[{time} {LevelCode(logLevel)}] {message}");
            }

            static string LevelCode(LogLevel level) => level switch
            {
                LogLevel.Information => "INF",
                LogLevel.Warning => "WRN",
                LogLevel.Error => "ERR",
                LogLevel.Critical => "FTL",
                LogLevel.Debug => "DBG",
                LogLevel.Trace => "VRB",
                _ => "INF",
            };
        }
    }
}
