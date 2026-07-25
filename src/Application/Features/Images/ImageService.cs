using CrossCutting.Settings;
using System.Net.Http.Headers;

namespace Application.Features.Images
{
    public class ImageService(HttpClient httpClient, ITemplateSettings settings)
    {
        public async Task UploadAsync(string name, Stream content, string contentType)
        {
            var contentTypeWithValue = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType;
            var requestContent = new StreamContent(content)
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue(contentTypeWithValue)
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, $"api/v1/images/{Uri.EscapeDataString(name)}")
            {
                Content = requestContent
            };
            request.Headers.Add("X-Api-Key", settings.ImageApiKey);

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public async Task<byte[]> GetAsync(string name)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/images/{Uri.EscapeDataString(name)}");
            request.Headers.Add("X-Api-Key", settings.ImageApiKey);

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        }

        public async Task DeleteAsync(string name)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, $"api/v1/images/{Uri.EscapeDataString(name)}");
            request.Headers.Add("X-Api-Key", settings.ImageApiKey);

            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        public string BuildUrl(string name)
        {
            return $"{settings.ImageApiUrl}/api/v1/images/{Uri.EscapeDataString(name)}";
        }
    }
}