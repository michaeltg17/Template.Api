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
            using var content = new MultipartFormDataContent();

            using var nameContent = new StringContent(request.Name);
            using var descriptionContent = new StringContent(request.Description);
            using var priceContent = new StringContent(request.Price.ToString());

            content.Add(nameContent, "name");
            content.Add(descriptionContent, "description");
            content.Add(priceContent, "price");

            StreamContent? imageContent = null;
            if (request.Image != null)
            {
                imageContent = new StreamContent(request.Image.OpenReadStream());
                content.Add(imageContent, "image", request.Image.FileName);
            }

            var response = await httpClient.PostAsync($"{BasePath}/Products", content);
            imageContent?.Dispose();
            return response;
        }

        public async Task<HttpResponseMessage> UpdateProduct(long id, UpdateProductRequest request)
        {
            return await UpdateProduct((object)id, request);
        }

        public async Task<HttpResponseMessage> UpdateProduct(object id, UpdateProductRequest request)
        {
            using var content = new MultipartFormDataContent();

            using var nameContent = new StringContent(request.Name);
            using var descriptionContent = new StringContent(request.Description);
            using var priceContent = new StringContent(request.Price.ToString());

            content.Add(nameContent, "name");
            content.Add(descriptionContent, "description");
            content.Add(priceContent, "price");

            StreamContent? imageContent = null;
            if (request.Image != null)
            {
                imageContent = new StreamContent(request.Image.OpenReadStream());
                content.Add(imageContent, "image", request.Image.FileName);
            }

            var response = await httpClient.PutAsync($"{BasePath}/Products/{id}", content);
            imageContent?.Dispose();
            return response;
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