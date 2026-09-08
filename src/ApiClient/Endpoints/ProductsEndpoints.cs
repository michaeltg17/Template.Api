using System.Net.Http.Json;
using Api.Features.Products.Models.Requests;
using static Api.Extensions.EndpointExtensions;

namespace ApiClient.Endpoints
{
    public class ProductsEndpoints(HttpClient httpClient)
    {
        public Task<HttpResponseMessage> GetAllProducts()
        {
            return httpClient.GetAsync(ProductsPath);
        }

        public Task<HttpResponseMessage> GetProduct(long id)
        {
            return GetProduct((object)id);
        }

        public Task<HttpResponseMessage> GetProduct(object id)
        {
            return httpClient.GetAsync($"{ProductsPath}/{id}");
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

            return await httpClient.PostAsync(ProductsPath, content);
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

            return await httpClient.PutAsync($"{ProductsPath}/{id}", content);
        }

        public async Task<HttpResponseMessage> DeleteProducts(DeleteProductsRequest request)
        {
            using var jsonRequest = new HttpRequestMessage(HttpMethod.Delete, ProductsPath)
            {
                Content = JsonContent.Create(request),
            };
            return await httpClient.SendAsync(jsonRequest);
        }
    }
}
