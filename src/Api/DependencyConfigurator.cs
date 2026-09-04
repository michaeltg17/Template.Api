using Api.Extensions;
using Api.Features.Images;
using Api.Features.Products;
using Application;
using CrossCutting;
using CrossCutting.Settings;
using Domain;
using FluentValidation;
using Persistence;
using Serilog;
using System.Linq.Expressions;
using System.Reflection;

namespace Api
{
    public static class DependencyConfigurator
    {
        public static WebApplicationBuilder AddDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

            builder.AddSerilog();

            builder.Services
                .AddMainDependencies()
                .AddProblemDetails();

            return builder;
        }

        public static IServiceCollection AddMainDependencies(this IServiceCollection services)
        {
            return services
                .AddApplicationDependencies()
                .AddDomainDependencies()
                .AddCrossCuttingDependencies()
                .AddPersistanceDependencies();
        }

        public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
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

        public static WebApplication Configure(this WebApplication app)
        {
            //Exception middleware first to catch exceptions
            app.UseExceptionHandler().UseStatusCodePages();

            app.MapEndpoints();

            return app;
        }

        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddScoped<ProductService>();
            services.AddHttpClient<ImageService>((sp, client) =>
            {
                var settings = sp.GetRequiredService<ITemplateApiSettings>();
                client.BaseAddress = settings.ImageApiUrl;
            });

            return services;
        }

        public static void ConfigureValidationWithCamelCase()
        {
            var defaultResolver = ValidatorOptions.Global.PropertyNameResolver;

            string camelCaseResolver(Type type, MemberInfo memberInfo, LambdaExpression expression)
            {
                var pascal = defaultResolver(type, memberInfo, expression);
                return string.Join(ValidatorOptions.Global.PropertyChainSeparator,
                    pascal.Split(ValidatorOptions.Global.PropertyChainSeparator, StringSplitOptions.None)
                        .Select(p => char.ToLowerInvariant(p[0]) + p[1..]));
            }

            ValidatorOptions.Global.PropertyNameResolver = camelCaseResolver;
            ValidatorOptions.Global.DisplayNameResolver = camelCaseResolver;
        }
    }
}
