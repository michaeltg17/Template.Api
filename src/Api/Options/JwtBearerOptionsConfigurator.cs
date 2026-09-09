using Api.Features.Auth;
using CrossCutting.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Api.Options
{
    public class JwtBearerOptionsConfigurator(ITemplateApiSettings settings) : IPostConfigureOptions<JwtBearerOptions>
    {
        public void PostConfigure(string? name, JwtBearerOptions options)
        {
            if (name != JwtBearerDefaults.AuthenticationScheme)
                return;

            var jwt = settings.Jwt;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwt.Issuer,
                ValidAudience = jwt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                NameClaimType = JwtTokenBuilder.NameClaim,
                RoleClaimType = JwtTokenBuilder.RoleClaim,
                ClockSkew = TimeSpan.Zero
            };

            options.MapInboundClaims = false;

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (!string.IsNullOrEmpty(context.Request.Cookies[jwt.CookieName]))
                    {
                        context.Token = context.Request.Cookies[jwt.CookieName];
                    }
                    else if (context.Request.Headers.Authorization is { } authorization
                        && authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Token = authorization["Bearer ".Length..];
                    }

                    return Task.CompletedTask;
                }
            };
        }
    }
}
