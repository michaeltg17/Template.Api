using Api.Exceptions;
using Api.Features.Auth.Extensions;
using Api.Features.Auth.Models.Requests;
using CrossCutting.Settings;
using Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Auth.Endpoints;

internal partial class LoginEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (
            [FromBody] LoginRequest request,
            IValidator<LoginRequest> validator,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ITemplateApiSettings settings,
            HttpContext httpContext,
            ILogger<LoginEndpoint> logger) =>
        {
            await validator.ValidateAndThrowAsync(request);

            var result = await signInManager.PasswordSignInAsync(
                request.Email, request.Password, isPersistent: true, lockoutOnFailure: true);
            if (!result.Succeeded)
                throw new UnauthorizedException("Invalid email or password.");

            var user = await userManager.GetUserAsync(httpContext.User);
            if (user is null)
                throw new UnauthorizedException("Unable to authenticate user.");

            var roles = await userManager.GetRolesAsync(user);
            var token = JwtTokenBuilder.CreateToken(user, roles, settings);

            httpContext.Response.Cookies.Append(
                settings.Jwt.CookieName, token, AuthCookie.BuildOptions(httpContext, settings));

            LogUserLoggedIn(logger, user.Id);
            return Results.Ok(user.ToAuthUserResponse(roles));
        });
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "User with id '{id}' logged in successfully.")]
    private static partial void LogUserLoggedIn(ILogger<LoginEndpoint> logger, string id);
}
