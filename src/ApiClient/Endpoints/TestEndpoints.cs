using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using static Api.Features.Test.TestEndpoints;

namespace ApiClient.Endpoints
{
    public class TestEndpoints(HttpClient httpClient)
    {
        const string BaseRoute = "/Test";

        public Task<HttpResponseMessage> ThrowInternalServerError()
        {
            return httpClient.PostAsync(new Uri($"{BaseRoute}/ThrowInternalServerError", UriKind.RelativeOrAbsolute), null);
        }

        public Task<HttpResponseMessage> GetOk()
        {
            return httpClient.GetAsync(new Uri($"{BaseRoute}/GetOk", UriKind.RelativeOrAbsolute));
        }

        public Task<HttpResponseMessage> Post(long id, DateTime date, PostRequest request)
        {
            return Post((object)id, date, request);
        }

        public Task<HttpResponseMessage> Post(object id, object? date, object? request)
        {
            var parameters = new Dictionary<string, string?>
            {
                { nameof(date), date?.ToString() }
            };

            var url = $"{BaseRoute}/Post/{id}" + QueryString.Create(parameters);

            return httpClient.PostAsJsonAsync(url, request);
        }

        public Task<HttpResponseMessage> RequestUnexistingRoute()
        {
            return httpClient.GetAsync(new Uri("UnexistingRoute/UnexistingRoute", UriKind.RelativeOrAbsolute));
        }
    }
}
