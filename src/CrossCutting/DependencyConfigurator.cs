using CrossCutting.Settings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            services.AddSingleton<ITemplateSettings>(services =>
            {
                var settings = services.GetRequiredService<IOptions<TemplateSettings>>().Value;
                var contentPath = services.GetRequiredService<IHostEnvironment>().ContentRootPath;
                settings.ImagesStoragePath = Path.Combine(contentPath, "images");
                Directory.CreateDirectory(settings.ImagesStoragePath);
                return settings;
            });

            return services;
        }
    }
}