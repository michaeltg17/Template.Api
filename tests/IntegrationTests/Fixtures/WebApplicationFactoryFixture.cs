using Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.XUnit.Injectable;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
using CrossCutting.Settings;
using IntegrationTests.Settings;
using IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Serilog.Sinks.XUnit.Injectable.Abstract;

namespace IntegrationTests.Fixtures
{
    internal abstract class WebApplicationFactoryFixture(
        ITestSettings testSettings,
        DatabaseFactory databaseFactory,
        string environment)
        : WebApplicationFactory<Program>, IAsyncLifetime
    {
        public InMemorySink InMemorySink { get; } = new();
        public InjectableTestOutputSink InjectableTestOutputSink { get; set; } = new();
        Database? Database { get; set; }

        private readonly object _lock = new();
        private readonly Dictionary<string, List<LogEvent>> _testLogs = new();

        internal List<LogEvent> GetTestLogs(string correlationId)
        {
            lock (_lock)
            {
                if (!_testLogs.TryGetValue(correlationId, out var list))
                {
                    list = new List<LogEvent>();
                    _testLogs[correlationId] = list;
                }
                return list;
            }
        }

        /// <summary>
        /// Enricher that reads the test correlation ID from AsyncLocal and adds it to every log event.
        /// Using AsyncLocal directly (not LogContext) ensures the value survives ConfigureAwait(false) boundaries.
        /// </summary>
        private sealed class CorrelationIdEnricher : ILogEventEnricher
        {
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory factory)
            {
                var correlationId = CorrelationIdStartupFilter.CorrelationId.Value;
                if (correlationId != null)
                {
                    logEvent.AddPropertyIfAbsent(factory.CreateProperty("__testCorrelationId", correlationId));
                }
            }
        }

        private sealed class CorrelatedInMemorySink : ILogEventSink
        {
            private readonly Dictionary<string, List<LogEvent>> _testLogs;
            private readonly object _lock;

            public CorrelatedInMemorySink(
                Dictionary<string, List<LogEvent>> testLogs,
                object @lock)
            {
                _testLogs = testLogs;
                _lock = @lock;
            }

            public void Emit(LogEvent logEvent)
            {
                if (logEvent.Properties.TryGetValue("__testCorrelationId", out var prop)
                    && prop is ScalarValue scalar
                    && scalar.Value is string correlationId)
                {
                    List<LogEvent> list;
                    lock (_lock)
                    {
                        if (!_testLogs.TryGetValue(correlationId, out list))
                        {
                            list = new List<LogEvent>();
                            _testLogs[correlationId] = list;
                        }
                    }
                    // Add outside lock — each test's list is only written by one test
                    list.Add(logEvent);
                }
            }
        }

        private CorrelatedInMemorySink? _correlatedSink;

        async ValueTask IAsyncLifetime.InitializeAsync()
        {
            Database = await databaseFactory.Create();
            _correlatedSink = new CorrelatedInMemorySink(_testLogs, _lock);
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment(environment);

            builder.UseSerilog((context, services, configuration) =>
            {
                Api.Startup.ApplyCommonSerilogConfiguration(context, services, configuration);
                configuration.WriteTo.Sink(InjectableTestOutputSink);
                configuration.WriteTo.Sink(_correlatedSink)
                    .Enrich.With<CorrelationIdEnricher>();
                configuration.WriteTo.Sink(InMemorySink);

                if (testSettings.EnableSqlLogging)
                {
                    configuration.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information);
                }
            });

            builder.ConfigureServices(services =>
            {
                services.AddHttpLogging(options =>
                    options.LoggingFields = HttpLoggingFields.RequestBody | HttpLoggingFields.ResponseBody);
                services.AddTransient<IStartupFilter, TestStartupFilter>();
                services.AddTransient<IStartupFilter, CorrelationIdStartupFilter>();
                services.AddSingleton<IInjectableTestOutputSink>(InjectableTestOutputSink);

                services.Configure<TemplateSettings>(templateSettings =>
                {
                    templateSettings.SqlServerConnectionString = Database!.ConnectionString;
                });

                if (testSettings.EnableSqlLogging)
                {
                    services.AddDbContext<AppDbContext>(options => options.EnableSensitiveDataLogging());
                }
            });

            return base.CreateHost(builder);
        }

        public async new Task DisposeAsync()
        {
            if (Database != null) await Database.DisposeAsync();
            await base.DisposeAsync();
        }
    }
}