using CrossCutting.Settings;
using IntegrationTests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Persistence;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using Serilog.Sinks.XUnit.Injectable;
using System.Net.Http;
using Xunit;
using Xunit.v3;

namespace IntegrationTests
{
    public abstract class Test : IAsyncLifetime
    {
        public ApiClient.ApiClient ApiClient { get; private set; } = default!;
        internal WebApplicationFactoryFixture WebApplicationFactoryFixture { get; set; } = default!;
        public ITestOutputHelper TestOutputHelper { get; set; } = default!;
        protected AppDbContext Context { get; set; } = default!;
        AsyncServiceScope Scope { get; set; } = default!;
        private string _correlationId = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Returns the list of log events captured only for this test instance.
        /// Uses correlation ID to isolate logs during parallel execution.
        /// </summary>
        protected List<LogEvent> LogEvents =>
            WebApplicationFactoryFixture.GetTestLogs(_correlationId);

        public virtual ValueTask Initialize()
        {
            WebApplicationFactoryFixture.InjectableTestOutputSink.Inject(TestOutputHelper);

            // Create HttpClient with the correlation handler wrapping the WebApplicationFactory's handler chain
            var client = WebApplicationFactoryFixture.CreateDefaultClient(
                new[] { new CorrelationIdHandler(_correlationId) });
            ApiClient = new(client);

            Scope = WebApplicationFactoryFixture.Services.CreateAsyncScope();
            Context = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
            return ValueTask.CompletedTask;
        }

        Task<int> DeleteEntitiesFromDb()
        {
            var sql = "TRUNCATE TABLE Products;";
            return Context.Database.ExecuteSqlRawAsync(sql);
        }

        void ClearImages()
        {
            var settings = Scope.ServiceProvider.GetRequiredService<ITemplateSettings>();
            if (Directory.Exists(settings.ImagesStoragePath))
            {
                Directory.Delete(settings.ImagesStoragePath, true);
                Directory.CreateDirectory(settings.ImagesStoragePath);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DeleteEntitiesFromDb();
            ClearImages();
            await Scope.DisposeAsync();
        }

        public ValueTask InitializeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// DelegatingHandler that adds X-Test-Correlation-Id to every outgoing request
    /// so the server-side middleware can tag logs per-test.
    /// </summary>
    internal sealed class CorrelationIdHandler : DelegatingHandler
    {
        private readonly string _correlationId;

        public CorrelationIdHandler(string correlationId)
        {
            _correlationId = correlationId;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Add("X-Test-Correlation-Id", _correlationId);
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}