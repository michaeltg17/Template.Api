using System.ComponentModel.DataAnnotations;

namespace FunctionalTests.Settings
{
    public class TestSettings : ITestSettings
    {
        [Required]
        public required Uri TemplateApiUrl { get; init; }
        public string? LoginEmail { get; init; }
        public string? LoginPassword { get; init; }
    }
}
