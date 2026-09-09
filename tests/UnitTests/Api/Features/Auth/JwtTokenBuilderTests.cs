using Api.Features.Auth;
using AwesomeAssertions;
using CrossCutting.Settings;
using Domain.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Xunit;

namespace UnitTests.Api.Features.Auth
{
    public class JwtTokenBuilderTests
    {
        static readonly ITemplateApiSettings Settings = new TemplateApiSettings
        {
            MaxImageSizeMb = 25,
            AllowedImageExtensions = [".jpg"],
            PostgreSqlConnectionString = "test",
            ImageApiUrl = new Uri("http://localhost"),
            ImageApiKey = "test-api-key",
            Jwt = new TemplateApiJwtSettings
            {
                Issuer = "test-issuer",
                Audience = "test-audience",
                Key = "test-jwt-key-must-be-long-enough-for-hs256-0123456789"
            },
            CorsOrigins = []
        };

        static readonly ApplicationUser User = new()
        {
            Id = "user-id",
            UserName = "user@test.com",
            Email = "user@test.com",
            FirstName = "Test",
            LastName = "User"
        };

        [Fact]
        public void CreateToken_ExpectedTokenValues()
        {
            var token = JwtTokenBuilder.CreateToken(User, [AuthRoles.User], Settings);
            var jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);

            jwt.Issuer.Should().Be("test-issuer");
            jwt.Audience.Should().Be("test-audience");
            jwt.ValidFrom.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
            jwt.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(4320), TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void CreateToken_ExpectedClaims()
        {
            var token = JwtTokenBuilder.CreateToken(User, [AuthRoles.User], Settings);
            var jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);

            jwt.GetPayloadValue<string>("sub").Should().Be("user-id");
            jwt.GetPayloadValue<string>("name").Should().Be("Test User");
            jwt.GetPayloadValue<string>("email").Should().Be("user@test.com");
            jwt.GetPayloadValue<List<string>>("role").Should().BeEquivalentTo([AuthRoles.User]);
        }

        [Fact]
        public void CreateTokenWithMultipleRoles_ExpectedAllRoleClaims()
        {
            var token = JwtTokenBuilder.CreateToken(User, [AuthRoles.User, AuthRoles.Admin], Settings);
            var jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);

            jwt.GetPayloadValue<List<string>>("role").Should().BeEquivalentTo([AuthRoles.User, AuthRoles.Admin]);
        }

        [Fact]
        public void CreateTokenWithoutRoles_ExpectedNoRoleClaim()
        {
            var token = JwtTokenBuilder.CreateToken(User, [], Settings);
            var jwt = new JsonWebTokenHandler().ReadJsonWebToken(token);

            jwt.Payload.Should().NotContainKey("role");
        }
    }
}
