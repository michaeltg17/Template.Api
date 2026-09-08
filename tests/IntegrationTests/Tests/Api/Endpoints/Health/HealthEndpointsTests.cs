using Api.Features.Health.Models.Responses;
using ApiClient.Extensions;
using AwesomeAssertions;
using IntegrationTests.Collections;
using IntegrationTests.Fixtures;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Health
{
    [Collection(nameof(DevelopmentApiCollectionFixture))]
    public class HealthEndpointsTests(TestFixture testFixture) : Test(testFixture)
    {
        [Fact]
        public async Task HealthLive_ReturnsOk()
        {
            //When
            var response = await ApiClient.Health.GetHealthLive();

            //Then
            var health = await response.To<HealthResponse>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            health.Status.Should().Be(HealthStatus.Healthy);
        }

        [Fact]
        public async Task HealthReady_ReturnsOk()
        {
            //When
            var response = await ApiClient.Health.GetHealthReady();

            //Then
            var health = await response.To<HealthResponse>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            health.Status.Should().Be(HealthStatus.Healthy);
        }
    }
}
