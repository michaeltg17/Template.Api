using Microsoft.Extensions.Options;

namespace CrossCutting.Settings
{
    internal class TemplateSettingsValidator : IValidateOptions<TemplateSettings>
    {
        public ValidateOptionsResult Validate(string? name, TemplateSettings templateSettings)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(templateSettings.SqlServerConnectionString))
                validationErrors.Add($"The '{nameof(templateSettings.SqlServerConnectionString)}' setting is required");

            if (templateSettings.ImageApiUrl is null or { IsAbsoluteUri: false })
                validationErrors.Add($"The '{nameof(templateSettings.ImageApiUrl)}' setting is required");

            if (string.IsNullOrWhiteSpace(templateSettings.ImageApiKey))
                validationErrors.Add($"The '{nameof(templateSettings.ImageApiKey)}' setting is required");

            if (validationErrors.Count > 0) return ValidateOptionsResult.Fail(validationErrors);

            return ValidateOptionsResult.Success;
        }
    }
}