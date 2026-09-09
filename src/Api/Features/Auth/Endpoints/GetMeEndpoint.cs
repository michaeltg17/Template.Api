using Api.Features.Auth.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Api.Features.Auth.Endpoints;

internal static class GetMeEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet("/me", async (
            ClaimsPrincipal principal,
            UserManager<ApplicationUser> userManager) =>
        {
            var sub = principal.FindFirst(JwtTokenBuilder.SubClaim)?.Value;
            var user = sub is null ? null : await userManager.FindByIdAsync(sub);

            if (user is null)
                return Results.Unauthorized();

            var roles = await userManager.GetRolesAsync(user);
            return Results.Ok(user.ToAuthUserResponse(roles));
        })
        .RequireAuthorization();
    }
}
