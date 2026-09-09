using Api.Features.Auth.Models.Requests;
using Api.Features.Auth.Models.Responses;
using ApiClient.Extensions;
using AwesomeAssertions;
using Core.Testing.Assertions;
using IntegrationTests.Collections;
using IntegrationTests.Fixtures;
using System.Net;
using Xunit;

namespace IntegrationTests.Tests.Api.Endpoints.Auth
{
    [Collection(nameof(DevelopmentApiCollectionFixture))]
    public class AuthEndpointsTests(TestFixture testFixture) : Test(testFixture)
    {
        protected const string BaseInstance = "/auth";

        [Fact]
        public async Task RegisterOk()
        {
            var email = $"register{Guid.NewGuid():N}@test.com";
            var request = new RegisterRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = email,
                Password = TestUserPassword
            };

            var response = await CreateAnonymousApiClient().Auth.Register(request);

            var user = await response.To<AuthUserResponse>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            user.Id.Should().NotBeNullOrEmpty();
            user.FirstName.Should().Be(request.FirstName);
            user.LastName.Should().Be(request.LastName);
            user.FullName.Should().Be($"{request.FirstName} {request.LastName}");
            user.Email.Should().Be(email);
            user.Roles.Should().BeEquivalentTo([AuthRoles.User]);

            var dbUser = await Context.Users.FindAsync(user.Id);
            dbUser.Should().NotBeNull();
            dbUser!.Email.Should().Be(email);
        }

        [Fact]
        public async Task RegisterDuplicateEmail_Expected400()
        {
            var request = new RegisterRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = $"duplicate{Guid.NewGuid():N}@test.com",
                Password = TestUserPassword
            };

            var firstResponse = await CreateAnonymousApiClient().Auth.Register(request);
            firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var secondResponse = await CreateAnonymousApiClient().Auth.Register(request);

            secondResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task RegisterAllPropertiesInvalid_ExpectedProblemDetails()
        {
            var response = await CreateAnonymousApiClient().Auth.Register(new RegisterRequest
            {
                FirstName = "",
                LastName = "",
                Email = "notanemail",
                Password = "short"
            });

            await ProblemDetailsAssertions.AssertValidationException(
                response,
                $"{BaseInstance}/register",
                new Dictionary<string, string[]>
                {
                    { "firstName", ["'firstName' must not be empty."] },
                    { "lastName", ["'lastName' must not be empty."] },
                    { "email", ["'email' is not a valid e-mail address."] },
                    { "password", ["'password' must not be shorter than 8 characters."] }
                });
        }

        [Fact]
        public async Task LoginOk()
        {
            var response = await CreateAnonymousApiClient().Auth.Login(new LoginRequest
            {
                Email = TestUserEmail,
                Password = TestUserPassword
            });

            var user = await response.To<AuthUserResponse>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            user.Id.Should().Be(TestUser.Id);
            user.Email.Should().Be(TestUserEmail);
            user.Roles.Should().BeEquivalentTo([AuthRoles.User]);

            var setCookie = response.Headers.GetValues("Set-Cookie").Single();
            setCookie.Should().Contain($"{CookieName}=");
        }

        [Fact]
        public async Task LoginInvalidCredentials_Expected401()
        {
            var response = await CreateAnonymousApiClient().Auth.Login(new LoginRequest
            {
                Email = TestUserEmail,
                Password = "WrongPassword1!"
            });

            await ProblemDetailsAssertions.AssertUnauthorizedException(
                response,
                $"{BaseInstance}/login",
                "Invalid email or password.");
        }

        [Fact]
        public async Task LogoutOk()
        {
            var response = await ApiClient.Auth.Logout();

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var setCookie = response.Headers.GetValues("Set-Cookie").Single();
            setCookie.Should().Contain($"{CookieName}=");
        }

        [Fact]
        public async Task LogoutWithoutToken_Expected401()
        {
            var response = await CreateAnonymousApiClient().Auth.Logout();

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetMeOk()
        {
            var response = await ApiClient.Auth.GetMe();

            var user = await response.To<AuthUserResponse>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            user.Id.Should().Be(TestUser.Id);
            user.Email.Should().Be(TestUserEmail);
            user.Roles.Should().BeEquivalentTo([AuthRoles.User]);
        }

        [Fact]
        public async Task GetMeWithoutToken_Expected401()
        {
            var response = await CreateAnonymousApiClient().Auth.GetMe();

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
