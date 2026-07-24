using Microsoft.Extensions.Options;

namespace CrossCutting.Settings
{
    internal class TemplateSettingsValidator : IValidateOptions<TemplateSettings>
    {
        public ValidateOptionsResult Validate(string? name, TemplateSettings templateSettings)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(templateSettings.ApiUrl))
                validationErrors.Add($"The '{nameof(templateSettings.ApiUrl)}' setting is required");

            if (string.IsNullOrWhiteSpace(templateSettings.ImagesRequestPath))
                validationErrors.Add($"The '{nameof(templateSettings.ImagesRequestPath)}' setting is required");

            if (string.IsNullOrWhiteSpace(templateSettings.SqlServerConnectionString))
                validationErrors.Add($"The '{nameof(templateSettings.SqlServerConnectionString)}' setting is required");

            if (validationErrors.Count > 0) return ValidateOptionsResult.Fail(validationErrors);

            return ValidateOptionsResult.Success;
        }
    }
}