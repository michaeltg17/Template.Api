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

            if (templateApiSettings.Jwt is null)
            {
                validationErrors.Add($"The '{nameof(templateApiSettings.Jwt)}' settings section is required");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(templateApiSettings.Jwt.Issuer))
                    validationErrors.Add($"The '{nameof(templateApiSettings.Jwt.Issuer)}' setting is required");

                if (string.IsNullOrWhiteSpace(templateApiSettings.Jwt.Audience))
                    validationErrors.Add($"The '{nameof(templateApiSettings.Jwt.Audience)}' setting is required");

                if (string.IsNullOrWhiteSpace(templateApiSettings.Jwt.Key))
                    validationErrors.Add($"The '{nameof(templateApiSettings.Jwt.Key)}' setting is required");
            }

            return validationErrors.Count > 0 ? ValidateOptionsResult.Fail(validationErrors) : ValidateOptionsResult.Success;
        }
    }
}