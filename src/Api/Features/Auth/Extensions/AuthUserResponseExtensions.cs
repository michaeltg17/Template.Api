using Api.Features.Auth.Models.Responses;
using Domain.Models;

namespace Api.Features.Auth.Extensions;

public static class AuthUserResponseExtensions
{
    public static AuthUserResponse ToAuthUserResponse(this ApplicationUser user, IEnumerable<string> roles)
    {
        return new AuthUserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            Roles = roles.ToArray()
        };
    }
}
