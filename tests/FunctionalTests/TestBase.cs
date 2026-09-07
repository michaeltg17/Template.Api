using FunctionalTests.Settings;

namespace FunctionalTests
{
    public abstract class TestBase
    {
        internal ApiClient.Client ApiClient { get; private set; } = default!;
        internal ITestSettings TestSettings { get; set; } = default!;

        internal void Initialize()
        {
            ApiClient = new ApiClient.Client(new HttpClient() { BaseAddress = TestSettings.TemplateApiUrl });
        }
    }
}
