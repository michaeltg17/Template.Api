using Api.Extensions;

namespace Api.Endpoints.Test
{
    public static class GetEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("Get/{id}", (
                long id,
                CancellationToken cancellationToken) =>
            {
                return Task.CompletedTask;
            })
            .WithTestName("Get")
            .WithOpenApi();
        }
    }
}
