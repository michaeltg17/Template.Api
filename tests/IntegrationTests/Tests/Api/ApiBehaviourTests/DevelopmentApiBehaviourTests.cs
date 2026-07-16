using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing;
using Core.Testing.Builders;
using Core.Testing.Validators;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.ApiBehaviourTests
{
    [Collection(nameof(DevelopmentApiCollection))]
    public class DevelopmentApiBehaviourTests : Test
    {
        [Fact]
        public async Task InternalServerError_ExposesSensitiveDetails()
        {
            //When
            var response = await ApiClient.Test.ThrowInternalServerError();

            //Then
            var problemDetails = await response.To<ProblemDetails>();
            var traceId = ProblemDetailsValidator.ValidateTraceId(problemDetails);

            var expected = new ProblemDetailsBuilder()
                .WithTraceId(traceId)
                .WithInternalServerErrorDevelopment("/Test/ThrowInternalServerError")
                .Build();

            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.ToLowerInvariant().Should().Contain("sensitive data".ToLowerInvariant());
            problemDetails.Should().BeEquivalentTo(expected);
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}