using Api.Features.Health;
using AwesomeAssertions;
using IntegrationTests.Collections;
using IntegrationTests.Fixtures;
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
            var client = TestFixture.WebApplicationFactory.CreateClient();
            var response = await client.GetAsync(new Uri(HealthEndpoints.LivePath, UriKind.Relative), TestContext.Current.CancellationToken);

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task HealthReady_WhenDbUp_ReturnsOk()
        {
            //When
            var client = TestFixture.WebApplicationFactory.CreateClient();
            var response = await client.GetAsync(new Uri(HealthEndpoints.ReadyPath, UriKind.Relative), TestContext.Current.CancellationToken);

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).Should().Be("Healthy");
        }
    }
}
