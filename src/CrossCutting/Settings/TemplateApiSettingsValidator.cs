using Microsoft.Extensions.Options;

namespace CrossCutting.Settings
{
    internal class TemplateApiSettingsValidator : IValidateOptions<TemplateApiSettings>
    {
        public ValidateOptionsResult Validate(string? name, TemplateApiSettings templateApiSettings)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(templateApiSettings.PostgreSqlConnectionString))
                validationErrors.Add($"The '{nameof(templateApiSettings.PostgreSqlConnectionString)}' setting is required");

            if (templateApiSettings.ImageApiUrl is null or { IsAbsoluteUri: false })
                validationErrors.Add($"The '{nameof(templateApiSettings.ImageApiUrl)}' setting is required");

            if (string.IsNullOrWhiteSpace(templateApiSettings.ImageApiKey))
                validationErrors.Add($"The '{nameof(templateApiSettings.ImageApiKey)}' setting is required");

            return validationErrors.Count > 0 ? ValidateOptionsResult.Fail(validationErrors) : ValidateOptionsResult.Success;
        }
    }
}