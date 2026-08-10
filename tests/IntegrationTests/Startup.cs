using IntegrationTests.Infrastructure;
using IntegrationTests.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.XUnit.Injectable;
using Xunit.DependencyInjection;

namespace IntegrationTests
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<BeforeAfterTest, BeforeAfterTestConfiguration>();
            services.AddScoped<InMemorySink>();
            services.AddScoped<InjectableTestOutputSink>();
            services.AddScoped<ImageApiMock>();
        }

        public static void ConfigureHost(IHostBuilder hostBuilder)
        {
            hostBuilder.AddConfiguration();
        }

        static IHostBuilder AddConfiguration(this IHostBuilder builder)
        {
            var testSettings = new Dictionary<string, string?>
            {
                {nameof(ITestSettings.EnableSqlLogging), "true"}
            };

            builder.ConfigureHostConfiguration(builder => builder.AddInMemoryCollection(testSettings));

            builder.ConfigureServices(services =>
            {
                services.AddOptions<TestSettings>().BindConfiguration("");
                services.AddSingleton<ITestSettings>(provider => provider.GetRequiredService<IOptions<TestSettings>>().Value);
            });

            return builder;
        }
    }
}