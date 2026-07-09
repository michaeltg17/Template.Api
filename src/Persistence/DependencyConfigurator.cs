using CrossCutting.Settings;
using Persistance.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence
{
    public static class DependencyConfigurator
    {
        public static IServiceCollection AddPersistanceDependencies(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>((provider, builder) =>
            {
                var apiSettings = provider.GetRequiredService<IApiSettings>();
                builder
                    .UseSqlServer(apiSettings.SqlServerConnectionString, sql => sql.EnableRetryOnFailure())
                    .AddInterceptors(new SetAuditInfoSaveChangesInterceptor());
            });

            return services;
        }
    }
}
