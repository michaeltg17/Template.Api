using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;

namespace Domain
{
    public static class DependencyConfigurator
    {
        public static IServiceCollection AddDomainDependencies(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
