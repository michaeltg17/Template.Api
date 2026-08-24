using IntegrationTests.Infrastructure;
using IntegrationTests.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Persistence.Migrations;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.XUnit.Injectable;
using Xunit.DependencyInjection;

namespace IntegrationTests
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<BeforeAfterTest, BeforeAfterTestConfiguration>();
            services.AddSingleton<InMemorySink>();
            services.AddSingleton<InjectableTestOutputSink>();
            services.AddScoped<ImageApiMock>();
            services.AddSingleton<DatabaseFactory>();
            services.AddSingleton<Migrator>();

            services.AddLogging();
            services.AddSingleton<ILoggerFactory, DiagnosticMessagesLoggerFactory>();

            services.AddOptions<TestSettings>().BindConfiguration("");
            services.AddSingleton<ITestSettings>(provider => provider.GetRequiredService<IOptions<TestSettings>>().Value);
        }
    }
}