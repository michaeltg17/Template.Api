using Api.Features.Health;
using AwesomeAssertions;
using IntegrationTests.Collections;
using IntegrationTests.Fixtures;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Health
{
    [Collection(nameof(UnhealthyApiCollectionFixture))]
    public class UnhealthyHealthEndpointsTests(TestFixture testFixture) : Test(testFixture)
    {
        [Fact]
        public async Task HealthReady_WhenUnhealthyCheck_ReturnsServiceUnavailable()
        {
            //When
            var client = TestFixture.WebApplicationFactory.CreateClient();
            var response = await client.GetAsync(new Uri(HealthEndpoints.ReadyPath, UriKind.Relative), TestContext.Current.CancellationToken);

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
            (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).Should().Be("Unhealthy");
        }

        [Fact]
        public async Task HealthLive_WhenUnhealthyCheck_ReturnsOk()
        {
            //When
            var client = TestFixture.WebApplicationFactory.CreateClient();
            var response = await client.GetAsync(new Uri(HealthEndpoints.LivePath, UriKind.Relative), TestContext.Current.CancellationToken);

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
