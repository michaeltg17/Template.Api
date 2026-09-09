using CrossCutting.Settings;
using Microsoft.AspNetCore.Http;

namespace Api.Features.Auth.Endpoints;

internal static class LogoutEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/logout", (HttpContext httpContext, ITemplateApiSettings settings) =>
        {
            httpContext.Response.Cookies.Delete(
                settings.Jwt.CookieName, AuthCookie.BuildOptions(httpContext, settings));

            return Results.Ok();
        })
        .RequireAuthorization();
    }
}
