using WireMock.Server;

namespace IntegrationTests.Extensions;

internal static class WireMockServerExtensions
{
    extension(WireMockServer server)
    {
        public Uri Uri => new(server.Url!);
    }
}