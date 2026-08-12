using ApiClient.Converters;
using ApiClient.Exceptions;
using ApiClient.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace ApiClient.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        static readonly ProblemDetailsValidator ProblemDetailsValidator = new();
        static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new NestedObjectConverter() }
        };

        public static async Task<T> To<T>(this HttpResponseMessage response)
        {
            ArgumentNullException.ThrowIfNull(response);

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
                throw new ApiClientException("Response content is null, empty or whitespace.");

            if (response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(ProblemDetails))
                {
                    var problemDetails = await GetValidProblemDetailsOrThrow(content);
                    return (T)(object)problemDetails;
                }
                else
                {
                    return JsonSerializer.Deserialize<T>(content, JsonSerializerOptions)!;
                }
            }
            else if (typeof(T) == typeof(ProblemDetails))
            {
                var problemDetails = await GetValidProblemDetailsOrThrow(content);
                return (T)(object)problemDetails;
            }
            else
            {
                var problemDetails = await GetValidProblemDetailsOrThrow(content);
                throw new ApiException(problemDetails);
            }
        }

        static async Task<ProblemDetails> GetValidProblemDetailsOrThrow(string content)
        {
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(content, JsonSerializerOptions)!;
            await ProblemDetailsValidator.ValidateAndThrowAsync(problemDetails);
            return problemDetails!;
        }

        public static async Task<T> To<T>(this Task<HttpResponseMessage> responseTask)
        {
            var response = await responseTask;
            return await response.To<T>();
        }

        public static async Task ValidateOrThrow(this HttpResponseMessage response, HttpStatusCode statusCode)
        {
            ArgumentNullException.ThrowIfNull(response);

            if (response.StatusCode != statusCode)
            {
                throw new ApiException(await response.To<ProblemDetails>());
            }
        }
    }
}
