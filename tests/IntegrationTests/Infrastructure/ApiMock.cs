using WireMock.Server;

namespace IntegrationTests.Infrastructure
{
    public abstract class ApiMock : IDisposable
    {
        public WireMockServer Server { get; }

        protected ApiMock()
        {
            Server = WireMockServer.Start();
        }

        public virtual void Dispose()
        {
            Server.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
