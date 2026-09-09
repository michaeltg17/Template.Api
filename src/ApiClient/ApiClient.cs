using System.Net.Http.Headers;
using ApiClient.Endpoints;

namespace ApiClient
{
    public class ApiClient
    {
        public TestEndpoints Test { get; }
        public ProductsEndpoints Products { get; }
        public HealthEndpoints Health { get; }
        public AuthEndpoints Auth { get; }

        public ApiClient(HttpClient httpClient, string? accessToken = null)
        {
            if (accessToken is not null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            Test = new(httpClient);
            Products = new(httpClient);
            Health = new(httpClient);
            Auth = new(httpClient);
        }
    }
}
