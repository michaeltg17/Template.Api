using ApiClient.Endpoints;

namespace ApiClient
{
    public class ApiClient(HttpClient httpClient)
    {
        public TestEndpoints Test { get; } = new(httpClient);
        public ProductsEndpoints Products { get; } = new(httpClient);
        public HealthEndpoints Health { get; } = new(httpClient);
    }
}
