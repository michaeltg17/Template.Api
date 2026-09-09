using System.Net.Http.Json;
using Api.Features.Auth.Models.Requests;
using static Api.Extensions.EndpointExtensions;

namespace ApiClient.Endpoints
{
    public class AuthEndpoints(HttpClient httpClient)
    {
        public Task<HttpResponseMessage> Register(RegisterRequest request)
        {
            return httpClient.PostAsJsonAsync($"{AuthPath}/register", request);
        }

        public Task<HttpResponseMessage> Login(LoginRequest request)
        {
            return httpClient.PostAsJsonAsync($"{AuthPath}/login", request);
        }

        public Task<HttpResponseMessage> Logout()
        {
            return httpClient.PostAsync($"{AuthPath}/logout", null);
        }

        public Task<HttpResponseMessage> GetMe()
        {
            return httpClient.GetAsync($"{AuthPath}/me");
        }
    }
}
