using Api.Features.Auth;
using ApiClient;
using AwesomeAssertions;
using CrossCutting.Settings;
using Domain.Models;
using IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Xunit;

namespace IntegrationTests
{
    public abstract class Test(TestFixture testFixture) : IAsyncLifetime
    {
        public const string ApiKey = "test-api-key";
        public const string Issuer = "IntegrationTests.Issuer";
        public const string Audience = "IntegrationTests.Audience";
        public const string JwtKey = "integration-tests-jwt-key-must-be-long-enough-for-hs256-0123456789";
        public const string CookieName = "bulletproof_react_app_token";
        public const string TestUserEmail = "test@template.api";
        public const string TestUserPassword = "Test1234!";

        public ApiClient.ApiClient ApiClient { get; private set; } = default!;
        protected AppDbContext Context { get; set; } = default!;
        AsyncServiceScope Scope { get; set; } = default!;
        protected HttpClient HttpClient { get; private set; } = default!;
        public TestFixture TestFixture { get; set; } = testFixture;
        protected ApplicationUser TestUser { get; private set; } = default!;

        public virtual async ValueTask Initialize(string? collectionFixtureName)
        {
            TestFixture.InjectableTestOutputSink.Inject(TestContext.Current.TestOutputHelper!);
            TestFixture.ImageApiMock!.Server.ResetLogEntries();
            TestFixture.SetWebApplicationFactory(collectionFixtureName);

            Scope = TestFixture.WebApplicationFactory.Services.CreateAsyncScope();
            Context = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
            HttpClient = Scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient();

            TestUser = await SeedTestUser();

            var userManager = Scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var settings = Scope.ServiceProvider.GetRequiredService<ITemplateApiSettings>();
            var roles = await userManager.GetRolesAsync(TestUser);
            var token = JwtTokenBuilder.CreateToken(TestUser, roles, settings);
            ApiClient = new(TestFixture.WebApplicationFactory.CreateClient(), token);
        }

        protected ApiClient.ApiClient CreateAnonymousApiClient()
        {
            return new(TestFixture.WebApplicationFactory.CreateClient());
        }

        private async Task<ApplicationUser> SeedTestUser()
        {
            var userManager = Scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var existing = await userManager.FindByEmailAsync(TestUserEmail);
            if (existing is not null)
                return existing;

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = TestUserEmail,
                Email = TestUserEmail,
                FirstName = "Test",
                LastName = "User"
            };

            var result = await userManager.CreateAsync(user, TestUserPassword);
            result.Succeeded.Should()
                .BeTrue(string.Join(", ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(user, AuthRoles.User);
            return user;
        }

        Task<int> DeleteEntitiesFromDb()
        {
            var sql = "TRUNCATE TABLE products RESTART IDENTITY;";
            return Context.Database.ExecuteSqlRawAsync(sql);
        }

        public async ValueTask DisposeAsync()
        {
            await DeleteEntitiesFromDb();
            await Scope.DisposeAsync();
            TestFixture.InMemorySink.Dispose();
            FlushLoggerAndValidateLogDone();
        }

        public ValueTask InitializeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public static void FlushLoggerAndValidateLogDone()
        {
            TestContext.Current.TestOutputHelper!.Output.Should().NotBeNullOrWhiteSpace();
        }
    }
}
