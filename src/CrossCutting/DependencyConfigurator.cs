using CrossCutting.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CrossCutting
{
    public static class DependencyConfigurator
    {
        public static IServiceCollection AddCrossCuttingDependencies(this IServiceCollection services)
        {
            services
                .AddOptionsWithValidateOnStart<TemplateApiSettings>()
                .BindConfiguration(ITemplateApiSettings.Section);

            services.AddSingleton<IValidateOptions<TemplateApiSettings>, TemplateApiSettingsValidator>();

            services.AddSingleton<ITemplateApiSettings>(sp => sp.GetRequiredService<IOptions<TemplateApiSettings>>().Value);

            return services;
        }
    }
}