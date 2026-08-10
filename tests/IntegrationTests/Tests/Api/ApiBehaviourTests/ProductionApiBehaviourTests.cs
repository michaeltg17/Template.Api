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
    [Collection(nameof(ProductionApiCollectionFixture))]
    public class ProductionApiBehaviourTests(TestFixture testFixture) : Test(testFixture)
    {
        [Fact]
        public async Task InternalServerError_HidesSensitiveData()
        {
            //When
            var response = await ApiClient.Test.ThrowInternalServerError();

            //Then
            var problemDetails = await response.To<ProblemDetails>();
            TraceIdValidator.IsValid(problemDetails.TraceId!).Should().BeTrue();

            var expected = new ProblemDetailsBuilder()
                .WithTraceId(problemDetails.TraceId!)
                .WithHiddenInternalServerError("/Test/ThrowInternalServerError")
                .Build();

            problemDetails.Should().BeEquivalentTo(expected);
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}