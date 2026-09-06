using WireMock.Server;

namespace IntegrationTests.Infrastructure
{
    public abstract class ApiMock : IDisposable
    {
        public WireMockServer Server { get; }
        bool disposed;

        protected ApiMock()
        {
            Server = WireMockServer.Start();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing) Server.Dispose();
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
