using IntegrationTests.Infrastructure;
using IntegrationTests.Settings;
using Microsoft.Extensions.DependencyInjection;
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

            services.AddOptions<TestSettings>().BindConfiguration("");
            services.AddSingleton<ITestSettings>(provider => provider.GetRequiredService<IOptions<TestSettings>>().Value);
        }
    }
}