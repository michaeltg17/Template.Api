using System.Net.Http.Json;
using ApiClient.Endpoints;
using Api.Extensions;
using Api.Features.Products.Models.Requests;

namespace ApiClient
{
    public class ApiClient(HttpClient httpClient)
    {
        public TestEndpoints Test { get; } = new(httpClient);

        const string BasePath = EndpointExtensions.BasePath;

        public Task<HttpResponseMessage> GetAllProducts()
        {
            return httpClient.GetAsync(new Uri($"{BasePath}/Products", UriKind.RelativeOrAbsolute));
        }

        public Task<HttpResponseMessage> GetProduct(long id)
        {
            return GetProduct((object)id);
        }

        public Task<HttpResponseMessage> GetProduct(object id)
        {
            return httpClient.GetAsync(new Uri($"{BasePath}/Products/{id}", UriKind.RelativeOrAbsolute));
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

            return await httpClient.PostAsync(new Uri($"{BasePath}/Products", UriKind.RelativeOrAbsolute), content);
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

            return await httpClient.PutAsync(new Uri($"{BasePath}/Products/{id}", UriKind.RelativeOrAbsolute), content);
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