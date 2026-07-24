using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using FluentValidation;

namespace Domain
{
    public static class DependencyConfigurator
    {
        public static IServiceCollection AddDomainDependencies(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblies([Assembly.GetExecutingAssembly()]);

            return services;
        }
    }
}
