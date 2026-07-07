using Application.Exceptions;
using Core.Extensions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Api.Extensions
{
    public static class ExceptionHandlerExtensions
    {
        public static WebApplication UseExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(config => config.Run(async httpContext =>
            {
                httpContext.Response.ContentType = "application/problem+json";
                var problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

                var exceptionHandlerFeature = httpContext.Features.GetRequiredFeature<IExceptionHandlerFeature>();
                var exception = exceptionHandlerFeature.Error;

                httpContext.Response.StatusCode = exception switch
                {
                    BadHttpRequestException => (int)HttpStatusCode.BadRequest,
                    NotFoundException => (int)HttpStatusCode.NotFound,
                    AppException => (int)HttpStatusCode.BadRequest,
                    _ => (int)HttpStatusCode.InternalServerError,
                };

                var problemDetailsContext = BuildProblemDetailsContext(exception, httpContext);

                if (exception is BadHttpRequestException bindingException)
                {
                    await HandleBindingFailure(httpContext, bindingException);
                }
                else
                {
                    await problemDetailsService.WriteAsync(problemDetailsContext);
                }
            }));

            return app;
        }

        static async Task HandleBindingFailure(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.ContentType = "application/problem+json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            string? paramName = null;
            string? paramValue = null;

            var message = exception.Message;

            if (exception.InnerException is JsonException jsonException)
            {
                paramName = "body";
                paramValue = jsonException.Message;
            }
            else if (message.StartsWith("Failed to bind parameter") && message.Contains(" from "))
            {
                var fromParts = message.Split(" from ");
                if (fromParts.Length >= 2)
                {
                    var paramPart = fromParts[0];
                    var valuePart = string.Join(" from ", fromParts.Skip(1));

                    var trimmedParam = paramPart["Failed to bind parameter ".Length..].Trim('"');
                    paramName = trimmedParam.Split().LastOrDefault() ?? "request";

                    var trimmedValue = valuePart.TrimStart('"').TrimEnd('"', '.');
                    paramValue = trimmedValue;
                }
            }

            if (paramName != null && paramValue != null)
            {
                await WriteValidationProblemDetails(httpContext, new Dictionary<string, string[]>
                {
                    { paramName, new[] { $"The value '{paramValue}' is not valid." } }
                });
                return;
            }

            if (message == "A non-empty request body is required.")
            {
                await WriteValidationProblemDetails(httpContext, new Dictionary<string, string[]>
                {
                    { "", new[] { "A non-empty request body is required." } }
                });
                return;
            }

            await WriteValidationProblemDetails(httpContext, new Dictionary<string, string[]>
            {
                { "request", new[] { message } }
            });
        }

        static async Task WriteValidationProblemDetails(HttpContext httpContext, IDictionary<string, string[]> errors)
        {
            var problemDetails = new ValidationProblemDetails(errors)
            {
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                Title = "ValidationException",
                Detail = "Please check the errors property for additional details.",
                Status = StatusCodes.Status400BadRequest,
                Instance = httpContext.Request.Path
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails);
        }

        static ProblemDetailsContext BuildProblemDetailsContext(Exception exception, HttpContext httpContext)
        {
            var isInternalServerError = httpContext.Response.StatusCode == (int)HttpStatusCode.InternalServerError;

            return new ProblemDetailsContext
            {
                Exception = isInternalServerError ? null : exception,
                HttpContext = httpContext,
                ProblemDetails =
                {
                    Title =  isInternalServerError ? "InternalServerError" : exception!.GetType().GetNameWithoutGenericArity(),
                    Detail = isInternalServerError ? "Internal server error. Please contact the API support." : exception!.Message,
                    Status = httpContext.Response.StatusCode,
                    Instance = httpContext.Request.Path
                }
            };
        }
    }
}
