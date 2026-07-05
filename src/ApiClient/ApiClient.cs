using ApiClient.Endpoints;

namespace ApiClient
{
    public class ApiClient(HttpClient httpClient)
    {
        public HttpClient HttpClient { get; } = httpClient;
        public ApiEndpoints Api { get; } = new(httpClient);
        public TestEndpoints Test { get; } = new(httpClient);

        public Task<HttpResponseMessage> RequestUnexistingRoute()
        {
            return HttpClient.GetAsync($"UnexistingRoute/UnexistingRoute");
        }
    }
}
