using ApiClient.Extensions;
using Core.Testing.Builders;
using Core.Testing.Extensions;
using AwesomeAssertions;
using Core.Testing;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Core.Testing.Validators
{
    public static class ProblemDetailsValidator
    {
        public static async Task ValidateNotAllFoundException(HttpResponseMessage response, string entity, string route, long[] ids)
        {
            var builder = new ProblemDetailsBuilder().WithNotAllFoundException(entity, route, ids);
            await ValidateCommon(response, builder);
        }

        public static async Task ValidateNotFoundException(HttpResponseMessage response, string entity, string route, long id)
        {
            var builder = new ProblemDetailsBuilder().WithNotFoundException(entity, route, id);
            await ValidateCommon(response, builder);
        }

        public static async Task ValidateValidationException(
            HttpResponseMessage response,
            string instance,
            IDictionary<string, string[]> expectedErrors)
        {
            var problemDetails = await response.To<ProblemDetails>();
            TraceIdValidator.IsValid(problemDetails.TraceId!).Should().BeTrue();
            var expected = new ProblemDetailsBuilder()
                .WithValidationException(instance, expectedErrors)
                .WithTraceId(problemDetails.TraceId!)
                .Build();
            problemDetails.Should().BeEquivalentTo(expected);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        static async Task ValidateCommon(HttpResponseMessage response, ProblemDetailsBuilder builder)
        {
            var problemDetails = await response.To<ProblemDetails>();
            TraceIdValidator.IsValid(problemDetails.TraceId!).Should().BeTrue();

            var expected = builder
                .WithTraceId(problemDetails.TraceId!)
                .Build();

            problemDetails.Should().BeEquivalentTo(expected);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
