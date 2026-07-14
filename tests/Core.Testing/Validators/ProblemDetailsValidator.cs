using ApiClient.Extensions;
using Core.Testing.Builders;
using Core.Testing.Extensions;
using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Core.Testing.Validators
{
    public static class ProblemDetailsValidator
    {
        public static string ValidateTraceId(ProblemDetails problemDetails)
        {
            var traceId = problemDetails.GetTraceId();
            TraceIdValidator.IsValid(traceId).Should().BeTrue();
            return traceId;
        }

        public static async Task ValidateNotFoundException(HttpResponseMessage response, string entity, string route, long[] ids)
        {
            var builder = new ProblemDetailsBuilder().WithNotFoundException(entity, route, ids);
            await ValidateNotFoundException(response, builder);
        }

        public static async Task ValidateValidationException(HttpResponseMessage response, string instance, string detail)
        {
            var problemDetails = await response.To<ProblemDetails>();
            var traceId = ValidateTraceId(problemDetails);
            var expected = new ProblemDetailsBuilder()
                .WithValidationException(instance, detail)
                .WithTraceId(traceId)
                .Build();
            problemDetails.Should().BeEquivalentTo(expected);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public static async Task ValidateNotFoundException(HttpResponseMessage response, string entity, string route, long id)
        {
            var builder = new ProblemDetailsBuilder().WithNotFoundException(entity, route, id);
            await ValidateNotFoundException(response, builder);
        }

        private static async Task ValidateNotFoundException(HttpResponseMessage response, ProblemDetailsBuilder builder)
        {
            var problemDetails = await response.To<ProblemDetails>();
            var traceId = ValidateTraceId(problemDetails);

            var expected = builder
                .WithTraceId(traceId)
                .Build();

            problemDetails.Should().BeEquivalentTo(expected);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
