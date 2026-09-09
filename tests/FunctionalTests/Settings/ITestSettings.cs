namespace FunctionalTests.Settings
{
    public interface ITestSettings
    {
        public Uri TemplateApiUrl { get; }
        public string? LoginEmail { get; }
        public string? LoginPassword { get; }
    }
}
