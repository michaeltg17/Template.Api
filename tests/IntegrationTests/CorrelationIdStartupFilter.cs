using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace IntegrationTests;

/// <summary>
/// Middleware that reads the <c>X-Test-Correlation-Id</c> header and stores
/// it in an AsyncLocal so downstream logs (even across ConfigureAwait(false)) are tagged per-test.
/// </summary>
internal class CorrelationIdStartupFilter : IStartupFilter
{
    internal static readonly AsyncLocal<string?> CorrelationId = new();

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            builder.Use(async (context, nextMiddleware) =>
            {
                if (context.Request.Headers.TryGetValue("X-Test-Correlation-Id", out var id)
                    && id.Count > 0
                    && !string.IsNullOrWhiteSpace(id[0]))
                {
                    CorrelationId.Value = id[0];
                    try
                    {
                        await nextMiddleware();
                    }
                    finally
                    {
                        CorrelationId.Value = null;
                    }
                }
                else
                {
                    await nextMiddleware();
                }
            });
            next(builder);
        };
    }
}