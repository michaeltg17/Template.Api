using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing.Builders;
using Core.Testing.Extensions;
using Core.Testing.Validators;
using IntegrationTests.Collections;
using IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.ApiBehaviourTests
{
    [Collection(nameof(DevelopmentApiCollectionFixture))]
    public class DevelopmentApiBehaviourTests(TestFixture testFixture) : Test(testFixture)
    {
        [Fact]
        public async Task InternalServerError_ExposesSensitiveData()
        {
            //When
            var response = await ApiClient.Test.ThrowInternalServerError();

            //Then
            var problemDetails = await response.To<ProblemDetails>();
            TraceIdValidator.IsValid(problemDetails.TraceId!).Should().BeTrue();
            ExceptionValidator.IsValid(problemDetails.Exception!).Should().BeTrue();

            var expected = new ProblemDetailsBuilder()
                .WithInternalServerError("Exception", "Sensitive data", "/Test/ThrowInternalServerError")
                .WithTraceId(problemDetails.TraceId!)
                .WithException(problemDetails.Exception!)
                .Build();

            problemDetails.Should().BeEquivalentTo(expected);
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}