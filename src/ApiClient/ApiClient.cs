using ApiClient.Endpoints;
using Application.Models.Requests;
using System.Net.Http.Json;

namespace ApiClient
{
    public class ApiClient(HttpClient httpClient)
    {
        public HttpClient HttpClient { get; } = httpClient;
        public TestEndpoints Test { get; } = new(httpClient);

        string BuildBasePath() => "/api";

        public Task<HttpResponseMessage> GetAllProducts()
        {
            return httpClient.GetAsync($"{BuildBasePath()}/Products");
        }

        public Task<HttpResponseMessage> GetProduct(long id)
        {
            return GetProduct((object)id);
        }

        public Task<HttpResponseMessage> GetProduct(object id)
        {
            return httpClient.GetAsync($"{BuildBasePath()}/Products/{id}");
        }

        public Task<HttpResponseMessage> CreateProduct(HttpContent? httpContent)
        {
            return httpClient.PostAsync($"{BuildBasePath()}/Products", httpContent);
        }

        public Task<HttpResponseMessage> UpdateProduct(long id, HttpContent? httpContent)
        {
            return UpdateProduct((object)id, httpContent);
        }

        public Task<HttpResponseMessage> UpdateProduct(object id, HttpContent? httpContent)
        {
            return httpClient.PutAsync($"{BuildBasePath()}/Products/{id}", httpContent);
        }

        public Task<HttpResponseMessage> DeleteProduct(long id)
        {
            return DeleteProduct((object)id);
        }

        public Task<HttpResponseMessage> DeleteProduct(object id)
        {
            return httpClient.DeleteAsync($"{BuildBasePath()}/Products/{id}");
        }

        public Task<HttpResponseMessage> Export(string tableName)
        {
            return httpClient.GetAsync($"{BuildBasePath()}/Export/{tableName}");
        }
    }
}