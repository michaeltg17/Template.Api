using IntegrationTests.Infrastructure;
using IntegrationTests.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit.DependencyInjection;

namespace IntegrationTests
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<BeforeAfterTest, BeforeAfterTestConfiguration>();
            services.AddSingleton<ProductionWebApplicationFactoryFixture>();
            services.AddSingleton<DevelopmentWebApplicationFactoryFixture>();
            services.AddSingleton<DatabaseFactory>();
        }

        public static void ConfigureHost(IHostBuilder hostBuilder)
        {
            hostBuilder.AddConfiguration();
        }

        static IHostBuilder AddConfiguration(this IHostBuilder builder)
        {
            var testSettings = new Dictionary<string, string?>
            {
                {nameof(ITestSettings.KeepAliveDatabase), "false"},
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
