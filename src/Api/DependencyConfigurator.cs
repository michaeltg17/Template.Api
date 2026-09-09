using Api.Extensions;
using Api.Features.Images;
using Api.Features.Products;
using Api.Options;
using CrossCutting;
using CrossCutting.Settings;
using Domain;
using Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Persistence;
using Serilog;
using System.Linq.Expressions;
using System.Reflection;

namespace Api
{
    public static class DependencyConfigurator
    {
        public const string CorsPolicyName = "frontend";

        public static WebApplicationBuilder AddDependencies(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);

            builder.AddSerilog();

            builder.Services
                .AddMainDependencies()
                .AddAuthDependencies()
                .AddProblemDetails();

            return builder;
        }

        public static IServiceCollection AddHealthCheckDependencies(this IServiceCollection services)
        {
            services
                .AddHealthChecks()
                .AddNpgSql(sp => sp.GetRequiredService<ITemplateApiSettings>().PostgreSqlConnectionString,
                    name: "db", timeout: TimeSpan.FromSeconds(5));

            return services;
        }

        public static IServiceCollection AddMainDependencies(this IServiceCollection services)
        {
            return services
                .AddApplicationDependencies()
                .AddDomainDependencies()
                .AddCrossCuttingDependencies()
                .AddPersistanceDependencies();
        }

        public static IServiceCollection AddAuthDependencies(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<AppDbContext>();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

            services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, JwtBearerOptionsConfigurator>();

            services.AddAuthorization();

            services.AddCors();
            services.AddSingleton<IPostConfigureOptions<CorsOptions>, CorsOptionsConfigurator>();

            return services;
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

            app.UseCors(CorsPolicyName);
            app.UseAuthentication();
            app.UseAuthorization();

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
