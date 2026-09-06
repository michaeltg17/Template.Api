using FunctionalTests.Settings;

namespace FunctionalTests
{
    public abstract class Test : IDisposable
    {
        internal ApiClient.ApiClient ApiClient { get; private set; } = default!;
        internal ITestSettings TestSettings { get; set; } = default!;
        HttpClient? httpClient;

        internal void Initialize()
        {
            httpClient = new HttpClient { BaseAddress = TestSettings.TemplateApiUrl };
            ApiClient = new ApiClient.ApiClient(httpClient);
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
