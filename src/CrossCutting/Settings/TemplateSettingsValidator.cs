using Microsoft.Extensions.Options;

namespace CrossCutting.Settings
{
    internal class TemplateSettingsValidator : IValidateOptions<TemplateSettings>
    {
        public ValidateOptionsResult Validate(string? name, TemplateSettings templateSettings)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(templateSettings.PostgreSqlConnectionString))
                validationErrors.Add($"The '{nameof(templateSettings.PostgreSqlConnectionString)}' setting is required");

            if (templateSettings.ImageApiUrl is null or { IsAbsoluteUri: false })
                validationErrors.Add($"The '{nameof(templateSettings.ImageApiUrl)}' setting is required");

            if (string.IsNullOrWhiteSpace(templateSettings.ImageApiKey))
                validationErrors.Add($"The '{nameof(templateSettings.ImageApiKey)}' setting is required");

            return validationErrors.Count > 0 ? ValidateOptionsResult.Fail(validationErrors) : ValidateOptionsResult.Success;
        }
    }
}