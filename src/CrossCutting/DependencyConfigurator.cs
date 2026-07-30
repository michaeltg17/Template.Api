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
                .AddOptionsWithValidateOnStart<TemplateSettings>()
                .BindConfiguration(ITemplateSettings.Section);

            services.AddSingleton<IValidateOptions<TemplateSettings>, TemplateSettingsValidator>();

            services.AddSingleton<ITemplateSettings>(sp => sp.GetRequiredService<IOptions<TemplateSettings>>().Value);

            return services;
        }
    }
}