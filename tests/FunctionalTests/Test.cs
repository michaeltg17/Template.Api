using Api.Features.Auth.Models.Requests;
using AwesomeAssertions;
using FunctionalTests.Settings;
using System.Net;

namespace FunctionalTests
{
    public abstract class Test
    {
        internal ApiClient.ApiClient ApiClient { get; private set; } = default!;
        internal ITestSettings TestSettings { get; set; } = default!;

        internal void Initialize()
        {
            HttpClient httpClient;
            if (TestSettings.LoginEmail is not null)
            {
                var handler = new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = new CookieContainer()
                };
                httpClient = new HttpClient(handler) { BaseAddress = TestSettings.TemplateApiUrl };
            }
            else
            {
                httpClient = new HttpClient() { BaseAddress = TestSettings.TemplateApiUrl };
            }

            ApiClient = new(httpClient);
        }

        internal async ValueTask LoginAsync()
        {
            if (TestSettings.LoginEmail is null)
                return;

            var response = await ApiClient.Auth.Login(
                new LoginRequest
                {
                    Email = TestSettings.LoginEmail,
                    Password = TestSettings.LoginPassword ?? string.Empty
                });

            response.StatusCode.Should()
                .Be(HttpStatusCode.OK, $"Login with '{TestSettings.LoginEmail}' should succeed, got {{statusCode}}.");
        }
    }
}
