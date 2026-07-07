using Core.Builders;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Net;

namespace Core.Testing.Builders
{
    public class ProblemDetailsBuilder : BuilderWithInstance<ProblemDetailsBuilder, ProblemDetails>
    {
        public ProblemDetailsBuilder() { Item.Extensions = new Dictionary<string, object?>(); }

        public ProblemDetailsBuilder WithTraceId(string traceId)
        {
            Item.Extensions.Add("traceId", traceId);
            return this;
        }

        public ProblemDetailsBuilder WithNotFound()
        {
            Item.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5";
            Item.Title = "Not Found";
            Item.Status = (int)HttpStatusCode.NotFound;

            return this;
        }

        public ProblemDetailsBuilder WithNotFoundException(string entity, long id)
        {
            Item.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5";
            Item.Title = "NotFoundException";
            Item.Status = (int)HttpStatusCode.NotFound;
            Item.Detail = $"{entity} with id '{id}' was not found.";
            Item.Instance = $"/api/v1/{entity}/{id}";

            return this;
        }

        public ProblemDetailsBuilder WithBadHttpRequestException()
        {
            Item.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
            Item.Title = "BadHttpRequestException";
            Item.Status = (int)HttpStatusCode.BadRequest;

            return this;
        }

        public ProblemDetailsBuilder WithInstance(string instance)
        {
            Item.Instance = instance;
            return this;
        }

        public ProblemDetailsBuilder WithDetail(string detail)
        {
            Item.Detail = detail;
            return this;
        }

        public ProblemDetailsBuilder WithInternalServerError(string instance)
        {
            Item.Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1";
            Item.Title = "InternalServerError";
            Item.Status = (int)HttpStatusCode.InternalServerError;
            Item.Detail = "Internal server error. Please contact the API support.";
            Item.Instance = instance;

            return this;
        }
    }
}
