namespace Api.Endpoints.Test
{
    internal static class ThrowInternalServerErrorEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("ThrowInternalServerError", () =>
            {
                throw new Exception("Sensitive data");
            });
        }
    }
}
