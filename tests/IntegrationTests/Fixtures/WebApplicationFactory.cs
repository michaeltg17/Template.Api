using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Serilog;
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
using IntegrationTests.Extensions;

namespace IntegrationTests.Fixtures
{
    public abstract class WebApplicationFactory(
        ITestSettings testSettings,
        InMemorySink inMemorySink,
        InjectableTestOutputSink injectableTestOutputSink,
        ImageApiMock imageApiMock,
        Database database)
        : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseSerilog((context, services, configuration) =>
            {
                Api.DependencyConfigurator.ApplyCommonSerilogConfiguration(context, services, configuration);
                configuration.WriteTo.Sink(injectableTestOutputSink);

                //Using Map sink to fix "Only first test is logged"
                configuration.WriteTo.Map(
                    _ => inMemorySink,
                    (_, writeTo) => writeTo.Sink(inMemorySink),
                    sinkMapCountLimit: 1);

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
                services.AddSingleton<IInjectableTestOutputSink>(injectableTestOutputSink);

                services.Configure<TemplateApiSettings>(templateSettings =>
                {
                    templateSettings.PostgreSqlConnectionString = database!.ConnectionString;
                    templateSettings.ImageApiUrl = imageApiMock!.Server.Uri;
                    templateSettings.ImageApiKey = Test.ApiKey;
                });

                if (testSettings.EnableSqlLogging)
                {
                    services.AddDbContext<AppDbContext>(options => options.EnableSensitiveDataLogging());
                }
            });

            return base.CreateHost(builder);
        }
    }
}