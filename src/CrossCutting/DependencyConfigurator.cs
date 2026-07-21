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
                .AddOptions<TemplateSettings>()
                .BindConfiguration(ITemplateSettings.Section)
                .PostConfigure(settings => settings.ImagesStoragePath = settings.ImagesStoragePath.Replace("%TEMP%", Path.GetTempPath()))
                .ValidateOnStart();

            services.AddSingleton<IValidateOptions<TemplateSettings>, TemplateSettingsValidator>();

            services.AddSingleton<ITemplateSettings>(provider => provider.GetRequiredService<IOptions<TemplateSettings>>().Value);

            return services;
        }
    }
}
