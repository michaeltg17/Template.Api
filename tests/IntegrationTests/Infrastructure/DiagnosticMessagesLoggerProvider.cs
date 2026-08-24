using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IntegrationTests.Infrastructure
{
    [SuppressMessage("Performance", "CA1801:Unused parameters", Justification = "ILoggerProvider implementation")]
    sealed class DiagnosticMessagesLoggerProvider : ILoggerProvider
    {
        static readonly DiagnosticLogger logger = new();

        public ILogger CreateLogger(string categoryName) => logger;

        public void Dispose()
        {
        }

        [SuppressMessage("Performance", "CA1801:Unused parameters", Justification = "ILogger implementation")]
        private sealed class DiagnosticLogger : ILogger
        {
            public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                if (!IsEnabled(logLevel)) return;
                var time = DateTimeOffset.Now.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
                var message = formatter(state, exception);
                if (exception is not null)
                {
                    message += "\n" + exception;
                }
                TestContext.Current.SendDiagnosticMessage($"[{time} {LevelCode(logLevel)}] {message}");
            }

            static string LevelCode(LogLevel level) => level switch
            {
                LogLevel.Trace => "VRB",
                LogLevel.Debug => "DBG",
                LogLevel.Information => "INF",
                LogLevel.Warning => "WRN",
                LogLevel.Error => "ERR",
                LogLevel.Critical => "FTL",
                LogLevel.None => "INF",
                _ => "INF",
            };
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            NullScope()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
