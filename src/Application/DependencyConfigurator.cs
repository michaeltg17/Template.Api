using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Reflection;
using Application.Services;
using Application.Validators;
using Domain.Validators;

namespace Application
{
    public static class DependencyConfigurator
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly()]);
            services.AddScoped<ProductService>();

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
