using CrossCutting.Settings;
using Domain.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Api.Features.Auth;

public static class JwtTokenBuilder
{
    public const string SubClaim = "sub";
    public const string NameClaim = "name";
    public const string EmailClaim = "email";
    public const string RoleClaim = "role";

    public static string CreateToken(ApplicationUser user, IEnumerable<string> roles, ITemplateApiSettings settings)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(settings);
        var jwt = settings.Jwt;

        var claims = new List<Claim>
        {
            new(SubClaim, user.Id),
            new(NameClaim, user.FullName ?? user.UserName),
            new(EmailClaim, user.Email ?? string.Empty)
        };
        claims.AddRange(roles.Select(role => new Claim(RoleClaim, role)));

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims),
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(jwt.ExpirationMinutes)
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}
