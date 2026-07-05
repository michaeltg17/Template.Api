using Api.Extensions;
using Api.Filters;

namespace Api.Endpoints.Test
{
    public static class GetOkEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("GetOk", (
                CancellationToken cancellationToken) =>
            {
                return Task.CompletedTask;
            })
            .WithTestName("GetOk")
            .WithOpenApi()
            .AddEndpointFilter<ValidationFilter>();
        }
    }
}
