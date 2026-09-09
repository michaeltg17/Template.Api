using CrossCutting.Settings;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace Api.Options
{
    public class CorsOptionsConfigurator(ITemplateApiSettings settings) : IPostConfigureOptions<CorsOptions>
    {
        public void PostConfigure(string? name, CorsOptions options)
        {
            options.AddPolicy(DependencyConfigurator.CorsPolicyName, policy =>
            {
                policy
                    .WithOrigins(settings.CorsOrigins.ToArray())
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithCredentials();
            });
        }
    }
}
