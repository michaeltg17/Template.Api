using Api.Features.Health;
using Api.Features.Health.Models.Responses;
using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing.Builders;
using Core.Testing.Extensions;
using Core.Testing.Validators;
using IntegrationTests.Collections;
using IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Health
{
    [Collection(nameof(UnhealthyApiCollectionFixture))]
    public class UnhealthyHealthEndpointsTests(UnhealthyTestFixture testFixture) : Test(testFixture)
    {
        protected override bool UsesDatabase => false;

        [Fact]
        public async Task HealthReady_WhenDbCheckFails_ReturnsServiceUnavailable()
        {
            //When
            var response = await ApiClient.Health.GetHealthReady();

            //Then
            var problemDetails = await response.To<ProblemDetails>();
            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            TraceIdValidator.IsValid(problemDetails.TraceId!).Should().BeTrue();

            var expected = new ProblemDetailsBuilder()
                .WithServiceUnavailable()
                .WithTraceId(problemDetails.TraceId!)
                .Build();

            problemDetails.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task HealthLive_WhenDbCheckFails_ReturnsOk()
        {
            //When
            var response = await ApiClient.Health.GetHealthLive();

            //Then
            var health = await response.To<HealthResponse>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            health.Status.Should().Be(HealthStatus.Healthy);
        }
    }
}
