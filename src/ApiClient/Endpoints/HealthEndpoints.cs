using static Api.Features.Health.HealthEndpoints;

namespace ApiClient.Endpoints
{
    public class HealthEndpoints(HttpClient httpClient)
    {
        public Task<HttpResponseMessage> GetHealthLive()
        {
            return httpClient.GetAsync(new Uri(LivePath, UriKind.Relative));
        }

        public Task<HttpResponseMessage> GetHealthReady()
        {
            return httpClient.GetAsync(new Uri(ReadyPath, UriKind.Relative));
        }
    }
}
