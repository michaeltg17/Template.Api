using Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using Serilog;
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

        async ValueTask IAsyncLifetime.InitializeAsync()
        {
            Database = await databaseFactory.Create();
        }

        /// <summary>
        /// To be called at the end of each test so that logs from previous test doesn't get mixed with the next one.
        /// </summary>
        public static void FlushLogger()
        {
            //Not the best but too hard to do it in another way.
            Thread.Sleep(10);
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment(environment);

            builder.UseSerilog((context, services, configuration) =>
            {
                Api.Startup.ApplyCommonSerilogConfiguration(context, services, configuration);
                configuration.WriteTo.Sink(InjectableTestOutputSink);

                //Using Map sink to fix "Only first test is logged"
                configuration.WriteTo.Map(
                    _ => InMemorySink,
                    (_, writeTo) => writeTo.Sink(InMemorySink),
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