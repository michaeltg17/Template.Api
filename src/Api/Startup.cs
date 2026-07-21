using Api.Extensions;
using Application;
using CrossCutting;
using CrossCutting.Settings;
using Microsoft.Extensions.FileProviders;
using Persistence;
using Serilog;

namespace Api
{
    public static class Startup
    {
        public static void Run(string[] args)
        {
            Application.DependencyConfigurator.ConfigureValidationWithCamelCase();

            WebApplication
                .CreateBuilder(args)
                .AddDependencies()
                .Build()
                .Configure()
                .Run();
        }

        static WebApplicationBuilder AddDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

            builder.AddSerilog();

            builder.Services
                .AddMainDependencies()
                .AddProblemDetails();

            return builder;
        }

        static IServiceCollection AddMainDependencies(this IServiceCollection services)
        {
            return services
                .AddApplicationDependencies()
                .AddCrossCuttingDependencies()
                .AddPersistanceDependencies();
        }

        static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                ApplyCommonSerilogConfiguration(context, services, configuration);
                configuration.WriteTo.Console();
            });

            return builder;
        }

        public static void ApplyCommonSerilogConfiguration(
            HostBuilderContext context, IServiceProvider services, LoggerConfiguration configuration)
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        }

        static WebApplication Configure(this WebApplication app)
        {
            //Exception middleware first to catch exceptions
            app.UseExceptionHandler().UseStatusCodePages();

            app.UseObjectStorage();

            app.MapEndpoints();

            return app;
        }

        static WebApplication UseObjectStorage(this WebApplication app)
        {
            var templateSettings = app.Services.GetRequiredService<ITemplateSettings>();
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = templateSettings.ImagesRequestPath,
                FileProvider = new PhysicalFileProvider(templateSettings.ImagesStoragePath)
            });

            return app;
        }
    }
}
