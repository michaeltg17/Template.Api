using CrossCutting.Settings;
using System.Net.Http.Headers;

namespace Application.Features.Images
{
    public class ImageService(HttpClient httpClient, ITemplateApiSettings settings)
    {
        public const string ImageApiKeyHeaderName = "X-Api-Key";
        public const string ApiPath = "images";

        public async Task Upload(string imageName, Stream content, string contentType)
        {
            var contentTypeWithValue = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType;
            var requestContent = new StreamContent(content)
            {
                Headers =
                {
                    ContentType = new MediaTypeHeaderValue(contentTypeWithValue)
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, BuildUrl(imageName))
            {
                Content = requestContent
            };
            request.Headers.Add(ImageApiKeyHeaderName, settings.ImageApiKey);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<byte[]> Get(string imageName)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, BuildUrl(imageName));
            request.Headers.Add(ImageApiKeyHeaderName, settings.ImageApiKey);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task Delete(string imageName)
        {
            using var request = new HttpRequestMessage(HttpMethod.Delete, BuildUrl(imageName));
            request.Headers.Add(ImageApiKeyHeaderName, settings.ImageApiKey);

            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public Uri BuildUrl(string imageName)
        {
            return BuildUrl(settings.ImageApiUrl, imageName);
        }

        public static Uri BuildUrl(Uri apiUrl, string imageName)
        {
            return new Uri(apiUrl, $"{ApiPath}/{imageName}");
        }

        public static Uri BuildPathUrl(string imageName)
        {
            return new Uri($"/{ApiPath}/{imageName}", UriKind.Relative);
        }
    }
}