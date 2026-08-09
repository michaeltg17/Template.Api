using WireMock.Server;

namespace IntegrationTests.Infrastructure
{
    internal abstract class ApiMock : IDisposable
    {
        public readonly WireMockServer Server;

        public ApiMock()
        {
            Server = WireMockServer.Start();
        }

        public void Dispose()
        {
            Server.Dispose();
        }
    }
}
