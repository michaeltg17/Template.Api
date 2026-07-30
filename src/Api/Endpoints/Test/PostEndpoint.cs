using Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Api.Endpoints.Test
{
    internal static class PostEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPost("Post/{id}", (
                long id,
                [FromQuery] DateTime date,
                [FromBody] PostRequest request,
                CancellationToken cancellationToken) =>
            {
                return Task.CompletedTask;
            });
        }
    }
}
