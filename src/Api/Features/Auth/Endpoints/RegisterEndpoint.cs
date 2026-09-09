using Api.Exceptions;
using Api.Features.Auth.Extensions;
using Api.Features.Auth.Models.Requests;
using Domain.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Features.Auth.Endpoints;

internal partial class RegisterEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/register", async (
            [FromBody] RegisterRequest request,
            IValidator<RegisterRequest> validator,
            UserManager<ApplicationUser> userManager,
            ILogger<RegisterEndpoint> logger) =>
        {
            await validator.ValidateAndThrowAsync(request);

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var error = result.Errors.Select(e => e.Description).FirstOrDefault() ?? "Failed to register user.";
                throw new TemplateApiException(error);
            }

            await userManager.AddToRoleAsync(user, AuthRoles.User);
            var roles = await userManager.GetRolesAsync(user);

            LogUserRegistered(logger, user.Id);
            return Results.Ok(user.ToAuthUserResponse(roles));
        });
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "User with id '{id}' registered successfully.")]
    private static partial void LogUserRegistered(ILogger<RegisterEndpoint> logger, string id);
}
