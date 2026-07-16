using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing;
using Core.Testing.Builders;
using Core.Testing.Validators;
using IntegrationTests.Collections;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.ApiBehaviourTests
{
    [Collection(nameof(ProductionApiCollection))]
    public class ProductionApiBehaviourTests : Test
    {
        [Fact]
        public async Task InternalServerError_HidesSensitiveData()
        {
            //When
            var response = await ApiClient.Test.ThrowInternalServerError();

            //Then
            var problemDetails = await response.To<ProblemDetails>();
            var traceId = ProblemDetailsValidator.ValidateTraceId(problemDetails);

            var expected = new ProblemDetailsBuilder()
                .WithTraceId(traceId)
                .WithHiddenInternalServerError("/Test/ThrowInternalServerError")
                .Build();

            var responseAsString = (await response.Content.ReadAsStringAsync()).ToLowerInvariant();
            responseAsString.Should().NotContain("Sensitive data".ToLowerInvariant());
            responseAsString.Should().NotContain("Exception".ToLowerInvariant());
            problemDetails.Should().BeEquivalentTo(expected);
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}