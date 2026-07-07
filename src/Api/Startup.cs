using CrossCutting;
using Persistence;
using Serilog;
using System.Text.Json.Serialization;
using Api.Extensions;
using Application;
using Asp.Versioning;

namespace Api
{
    public static class Startup
    {
        public static void Run(string[] args)
        {
            WebApplication
                .CreateBuilder(args)
                .AddDependencies()
                .Build()
                .Configure()
                .Run();
        }

        static WebApplicationBuilder AddDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => 
                options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

            builder.AddSerilog();

            builder.Services
                .AddMainDependencies()
                .AddProblemDetails()
                .AddVersioning();

            return builder;
        }

        static IServiceCollection AddVersioning(this IServiceCollection services)
        {
            return services
                .AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;
                    options.DefaultApiVersion = new ApiVersion(1);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                })
                .AddMvc()
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'V";
                    options.SubstituteApiVersionInUrl = true;
                })
                .Services;
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

            app.MapEndpoints();

            app.LogEndpoints();

            return app;
        }
    }
}
