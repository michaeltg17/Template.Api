namespace Api.Endpoints.Test
{
    internal static class GetOkEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("GetOk", (
                CancellationToken cancellationToken) =>
            {
                return Task.CompletedTask;
            });
        }
    }
}
