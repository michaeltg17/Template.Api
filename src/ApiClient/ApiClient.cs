using System.Net.Http.Json;
using ApiClient.Endpoints;
using Api.Extensions;
using Api.Features.Health;
using Api.Features.Products.Models.Requests;

namespace ApiClient
{
    public class ApiClient(HttpClient httpClient)
    {
        public TestEndpoints Test { get; } = new(httpClient);

        const string BasePath = EndpointExtensions.BasePath;

        public Task<HttpResponseMessage> GetHealthLive()
        {
            return httpClient.GetAsync(new Uri(HealthEndpoints.LivePath, UriKind.Relative));
        }

        public Task<HttpResponseMessage> GetHealthReady()
        {
            return httpClient.GetAsync(new Uri(HealthEndpoints.ReadyPath, UriKind.Relative));
        }

        public Task<HttpResponseMessage> GetAllProducts()
        {
            return httpClient.GetAsync($"{BasePath}/Products");
        }

        public Task<HttpResponseMessage> GetProduct(long id)
        {
            return GetProduct((object)id);
        }

        public Task<HttpResponseMessage> GetProduct(object id)
        {
            return httpClient.GetAsync($"{BasePath}/Products/{id}");
        }

        public async Task<HttpResponseMessage> CreateProduct(CreateProductRequest request)
        {
            using var content = new MultipartFormDataContent
            {
                { new StringContent(request.Name), "name" },
                { new StringContent(request.Description), "description" },
                { new StringContent(request.Price.ToString()), "price" }
            };

            if (request.Image != null)
            {
                content.Add(new StreamContent(request.Image.OpenReadStream()), "image", request.Image.FileName);
            }

            return await httpClient.PostAsync($"{BasePath}/Products", content);
        }

        public async Task<HttpResponseMessage> UpdateProduct(long id, UpdateProductRequest request)
        {
            return await UpdateProduct((object)id, request);
        }

        public async Task<HttpResponseMessage> UpdateProduct(object id, UpdateProductRequest request)
        {
            using var content = new MultipartFormDataContent
            {
                { new StringContent(request.Name), "name" },
                { new StringContent(request.Description), "description" },
                { new StringContent(request.Price.ToString()), "price" }
            };

            if (request.Image != null)
            {
                content.Add(new StreamContent(request.Image.OpenReadStream()), "image", request.Image.FileName);
            }

            return await httpClient.PutAsync($"{BasePath}/Products/{id}", content);
        }

        public async Task<HttpResponseMessage> DeleteProducts(DeleteProductsRequest request)
        {
            using var jsonRequest = new HttpRequestMessage(HttpMethod.Delete, $"{BasePath}/Products")
            {
                Content = JsonContent.Create(request),
            };
            return await httpClient.SendAsync(jsonRequest);
        }
    }
}