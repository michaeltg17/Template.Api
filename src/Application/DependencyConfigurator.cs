using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Reflection;
using Application.Features.Images;
using Application.Features.Products.Actions;
using CrossCutting.Settings;

namespace Application
{
    public static class DependencyConfigurator
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddScoped<ProductService>();
            services.AddScoped<GetProductByIdQuery>();
            services.AddScoped<GetAllProductsQuery>();
            services.AddScoped<CreateProductCommand>();
            services.AddScoped<UpdateProductCommand>();
            services.AddScoped<DeleteProductsCommand>();
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