using CrossCutting.Settings;
using Microsoft.AspNetCore.Http;

namespace Api.Features.Auth;

public static class AuthCookie
{
    public static CookieOptions BuildOptions(HttpContext httpContext, ITemplateApiSettings settings)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps,
            SameSite = httpContext.Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddMinutes(settings.Jwt.ExpirationMinutes)
        };
    }
}
