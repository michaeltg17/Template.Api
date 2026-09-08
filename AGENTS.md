# Template.Api

.NET 10 ASP.NET Core API template with projects: Api, ApiClient, Core, CrossCutting, Domain, Persistence, and Persistence.Migrations.

## Architecture

- **Api** — ASP.NET Core minimal API. Business logic (services, exceptions, request/response models) lives here under `Features/`. The former `Application` project was removed.
- **ApiClient** — HTTP client library with endpoints, extensions, exceptions, and validators
- **Core** — Builder pattern and helpers
- **CrossCutting** — Shared concerns: settings and DI configurator
- **Domain** — Anemic domain entities (EF Core models) and validators
- **Persistence** — EF Core DbContext, configurations, and data access
- **Persistence.Migrations** — dbup-based SQL migration runner

DI is composed via `DependencyConfigurator` classes: each project exposes `Add*Dependencies()` extension methods (e.g. `AddDomainDependencies`, `AddCrossCuttingDependencies`, `AddPersistanceDependencies`). `Api/DependencyConfigurator.AddDependencies()` wires everything together (Serilog, main dependencies, ProblemDetails), and `Configure()` adds the exception handler and maps endpoints.

## Structure

```
├── .dockerignore
├── .editorconfig
├── .env.example                    # sample env vars for docker compose
├── .gitattributes
├── .gitignore
├── AGENTS.md
├── ARCHITECTURE.md                 # architecture narrative
├── ci-docker.sh                    # CI docker run script
├── ci.sh                           # CI entrypoint script
├── Directory.Build.props           # shared props: net10.0, nullable, implicit usings
├── Directory.Packages.props        # central package versions
├── README.md
├── Template.Api.slnx
├── docker-compose.yml              # postgresql, migrator, api services (reads .env)
├── Dockerfile                      # multi-stage: SDK build → runtime (aspnet:10.0)
├── Dockerfile.ci                   # CI runtime image with test dependencies
├── Dockerfile.migrations           # multi-stage: SDK build → runtime for migrator
├── .github/workflows/              # GH Actions
│   └── ci.yml
├── src/
│   ├── Api/                        # ASP.NET Core minimal API
│   │   ├── Api.csproj
│   │   ├── Api.http
│   │   ├── Program.cs              # entrypoint (CreateBuilder().AddDependencies().Build().Configure().Run())
│   │   ├── DependencyConfigurator.cs  # DI: AddDependencies, AddMainDependencies, AddSerilog, AddApplicationDependencies, Configure
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── Exceptions/
│   │   │   ├── TemplateApiException.cs       # base exception
│   │   │   ├── NotFoundException.cs
│   │   │   ├── NotFoundException(T).cs
│   │   │   ├── NotAllFoundException.cs
│   │   │   └── NotAllFoundException(T).cs
│   │   ├── Extensions/
│   │   │   ├── EndpointExtensions.cs         # MapEndpoints, MapGroup, route path constants
│   │   │   ├── ExceptionHandlerExtensions.cs # exception → ProblemDetails mapping
│   │   │   └── ValidationFailureExtensions.cs
│   │   ├── Features/
│   │   │   ├── Health/
│   │   │   │   └── HealthEndpoints.cs        # /health/live (no checks) + /health/ready (db check)
│   │   │   ├── Images/
│   │   │   │   └── ImageService.cs           # HttpClient for the external image API
│   │   │   ├── Products/
│   │   │   │   ├── ProductService.cs         # image ops, validation, URL building
│   │   │   │   ├── Endpoints/
│   │   │   │   │   ├── CreateProductEndpoint.cs
│   │   │   │   │   ├── DeleteProductsEndpoint.cs
│   │   │   │   │   ├── GetAllProductsEndpoint.cs
│   │   │   │   │   ├── GetProductEndpoint.cs
│   │   │   │   │   └── UpdateProductEndpoint.cs
│   │   │   │   └── Models/
│   │   │   │       ├── Requests/
│   │   │   │       │   ├── CreateProductRequest.cs
│   │   │   │       │   ├── DeleteProductsRequest.cs
│   │   │   │       │   ├── UpdateProductRequest.cs
│   │   │   │       │   └── Validators/
│   │   │   │       │       ├── CreateProductRequestValidator.cs
│   │   │   │       │       └── DeleteProductsRequestValidator.cs
│   │   │   │       └── Responses/
│   │   │   │           └── DeleteProductsResponse.cs
│   │   │   └── Test/
│   │   │       └── TestEndpoints.cs          # GetOk, Post/{id}, ThrowInternalServerError
│   │   └── Properties/
│   │       └── launchSettings.json
│   ├── ApiClient/                  # HTTP client library
│   │   ├── ApiClient.csproj
│   │   ├── ApiClient.cs
│   │   ├── Converters/
│   │   │   └── NestedObjectConverter.cs
│   │   ├── Endpoints/
│   │   │   └── TestEndpoints.cs
│   │   ├── Exceptions/
│   │   │   ├── ApiClientException.cs
│   │   │   └── ApiException.cs
│   │   ├── Extensions/
│   │   │   ├── HttpResponseMessageExtensions.cs
│   │   │   └── ProblemDetailsExtensions.cs
│   │   └── Validators/
│   │       └── ProblemDetailsValidator.cs
│   ├── Core/                       # builder pattern and helpers
│   │   ├── Core.csproj
│   │   ├── Builders/
│   │   │   ├── Builder.cs
│   │   │   ├── BuilderWithInstance.cs
│   │   │   ├── BuilderWithValues.cs
│   │   │   └── IBuilder.cs
│   │   └── Extensions/
│   │       ├── StringExtensions.cs
│   │       └── TypeExtensions.cs       # helper for type names
│   ├── CrossCutting/               # shared concerns
│   │   ├── CrossCutting.csproj
│   │   ├── DependencyConfigurator.cs   # settings binding + validation
│   │   └── Settings/
│   │       ├── ITemplateApiSettings.cs
│   │       ├── TemplateApiSettings.cs     # POCO bound from config
│   │       └── TemplateApiSettingsValidator.cs # IValidateOptions for settings
│   ├── Domain/                     # domain entities
│   │   ├── Domain.csproj
│   │   ├── DependencyConfigurator.cs   # registers domain validators
│   │   ├── Models/
│   │   │   ├── Entity.cs
│   │   │   ├── Image.cs
│   │   │   └── Product.cs
│   │   └── Validators/
│   │       └── ProductValidator.cs
│   ├── Persistence/                # EF Core data access
│   │   ├── Persistence.csproj
│   │   ├── DependencyConfigurator.cs   # AddDbContext
│   │   ├── AppDbContext.cs
│   │   └── Configurations/
│   │       ├── EntityConfiguration.cs
│   │       └── ProductConfiguration.cs
│   └── Persistence.Migrations/     # dbup SQL migration runner
│       ├── Persistence.Migrations.csproj
│       ├── Program.cs
│       ├── Migrator.cs
│       ├── Extensions/
│       │   └── DatabaseUpgradeResultExtensions.cs
│       └── Scripts/
│           ├── 0001_Initial.sql
│           ├── 0002_AddImageName.sql
│           └── 0003_RenameImageNameToFileName.sql
└── tests/
    ├── Core.Testing/               # shared test utilities (builders, validators)
    │   ├── Core.Testing.csproj
    │   ├── Assertions/
    │   │   └── ProblemDetailsAssertions.cs
    │   ├── Builders/
    │   │   ├── CreateProductRequestBuilder.cs
    │   │   ├── DeleteProductsRequestBuilder.cs
    │   │   ├── DeleteProductsResponseBuilder.cs
    │   │   ├── ProblemDetailsBuilder.cs
    │   │   ├── ProductBuilder.cs
    │   │   └── UpdateProductRequestBuilder.cs
    │   ├── Extensions/
    │   │   └── ProblemDetailsExtensions.cs
    │   ├── Images/
    │   │   ├── didi.jpeg
    │   │   └── didi2.jpg
    │   ├── Serializers/
    │   │   └── TestCaseSerializer.cs
    │   └── Validators/
    │       ├── ExceptionValidator.cs
    │       └── TraceIdValidator.cs
    ├── UnitTests/                  # isolated unit tests (xunit.v3, AwesomeAssertions)
    │   ├── UnitTests.csproj
    │   ├── Application/Features/Products/Models/Requests/Validators/
    │   │   └── DeleteProductsRequestValidatorTests.cs
    │   ├── Core/
    │   │   └── TypeExtensionsTests.cs
    │   ├── Core/Testing/
    │   │   ├── Serializers/
    │   │   │   └── TestCaseSerializerTests.cs
    │   │   └── Validators/
    │   │       ├── ExceptionValidatorTests.cs
    │   │       └── TraceIdValidatorTests.cs
    │   └── Domain/Validators/
    │       └── ProductValidatorTests.cs
    ├── IntegrationTests/           # WebApplicationFactory + Testcontainers.PostgreSql + WireMock
    │   ├── IntegrationTests.csproj
    │   ├── BeforeAfterTestConfiguration.cs
    │   ├── Startup.cs
    │   ├── Test.cs
    │   ├── IntegrationTestsException.cs
    │   ├── TestStartupFilter.cs
    │   ├── xunit.runner.json
    │   ├── Settings/
    │   │   ├── ITestSettings.cs
    │   │   └── TestSettings.cs
    │   ├── Collections/
    │   │   ├── DevelopmentApiCollectionFixture.cs
    │   │   ├── ProductionApiCollectionFixture.cs
    │   │   └── UnhealthyApiCollectionFixture.cs
    │   ├── Extensions/
    │   │   ├── LogEventPropertyAssertionExtensions.cs
    │   │   ├── ObjectExtensions.cs
    │   │   └── WireMockServerExtensions.cs
    │   ├── Fixtures/
    │   │   ├── WebApplicationFactory.cs
    │   │   ├── DevelopmentWebApplicationFactory.cs
    │   │   ├── ProductionWebApplicationFactory.cs
    │   │   ├── UnhealthyWebApplicationFactory.cs
    │   │   └── TestFixture.cs
    │   ├── Infrastructure/
    │   │   ├── ApiMock.cs          # WireMock base for external APIs
    │   │   ├── ImageApiMock.cs
    │   │   ├── Database.cs
    │   │   ├── DatabaseFactory.cs
    │   │   └── DiagnosticMessagesLoggerProvider.cs
    │   └── Tests/
    │       ├── Api/
    │       │   ├── ApiBehaviourTests/
    │       │   │   ├── CommonApiBehaviourTests.cs
    │       │   │   ├── DevelopmentApiBehaviourTests.cs
    │       │   │   ├── ProductionApiBehaviourTests.cs
    │       │   │   └── BadRequestTests.cs
    │       │   ├── Endpoints/Health/
    │       │   │   ├── HealthEndpointsTests.cs
    │       │   │   └── UnhealthyHealthEndpointsTests.cs
    │       │   └── Endpoints/Products/
    │       │       ├── ProductsTest.cs
    │       │       ├── CreateProductEndpointTests.cs
    │       │       ├── GetProductEndpointTests.cs
    │       │       ├── GetAllProductsEndpointTests.cs
    │       │       ├── UpdateProductEndpointTests.cs
    │       │       └── DeleteProductsEndpointTests.cs
    │       └── ApiClient/
    │           └── ApiClientTests.cs
    ├── FunctionalTests/            # E2E tests against live API (requires docker-compose)
    │   ├── FunctionalTests.csproj
    │   ├── BeforeAfterTestConfiguration.cs
    │   ├── Startup.cs
    │   ├── Test.cs
    │   ├── Settings/
    │   │   ├── ITestSettings.cs
    │   │   ├── TestSettings.cs
    │   │   └── testsettings.json
    │   └── Tests/
    │       └── Products/
    │           └── GetTest.cs
    └── Persistence.Migrations.Tests/  # migrator tests against Testcontainers.PostgreSql
        ├── Persistence.Migrations.Tests.csproj
        └── MigratorTests.cs
```

## Configuration

App settings bind to `TemplateApiSettings` under the `TemplateApi` config section via `CrossCutting/DependencyConfigurator.AddCrossCuttingDependencies()`. Validated at startup via `TemplateApiSettingsValidator` using `IValidateOptions`. Application fails to start if required settings are missing.

`Program.cs` is a minimal entrypoint: it configures the camelCase FluentValidation property-name resolver, then runs `WebApplication.CreateBuilder(args).AddDependencies().Build().Configure().Run()`. All DI, Serilog, and endpoint registration happen in `Api/DependencyConfigurator.cs`.

## Endpoints

Endpoints are organized under `src/Api/Features/<Feature>/Endpoints/` by feature. Each endpoint is a `static class` with a `Map(IEndpointRouteBuilder)` method, registered in `EndpointExtensions.MapEndpoints()` (products under the `api/products` group, test endpoints under `Test`).

Health endpoints (mapped in `HealthEndpoints`): `/health/live` runs no checks (always 200 while the process is up) and `/health/ready` runs the PostgreSQL `db` check (200 Healthy / 503 Unhealthy). Checks are registered in `DependencyConfigurator.AddHealthCheckDependencies()`. Intended for k8s liveness/readiness probes and load-balancer health checks.

Responses use `application/problem+json`. Invalid requests return 400, other errors return 500 with details hidden in production.

## Build & Run

```bash
cp .env.example .env                # then adjust values
dotnet run --project src/Api
# or full stack via docker compose (postgresql → migrator → api):
docker compose up
```

SQL migrations run via the `Persistence.Migrations` project (dbup) and are executed by the `migrator` service in `docker-compose.yml`.

## Tests

```bash
dotnet test
```

Four test projects: `UnitTests`, `IntegrationTests`, `FunctionalTests`, `Persistence.Migrations.Tests`, plus `Core.Testing` for shared utilities.

- **UnitTests** — isolated unit tests (xunit.v3, AwesomeAssertions); no mocking
- **IntegrationTests** — `WebApplicationFactory` + `Testcontainers.PostgreSql`; the external image API is mocked with `WireMock.Net` (`Infrastructure/ImageApiMock`). Runs against Development and Production environment fixtures (requires Docker socket)
- **Persistence.Migrations.Tests** — dbup migrator tests against `Testcontainers.PostgreSql`
- **FunctionalTests** — E2E tests against a live API; requires `docker compose up` and `ApiUrl` in `Settings/testsettings.json`

CI runs via `ci-docker.sh` which builds `Dockerfile.ci` and mounts the Docker socket to enable Testcontainers.

## Logging Overrides

Serilog level overrides in `appsettings.json` under `MinimumLevel.Override`. Known overrides to silence noisy defaults:

- `"Microsoft.EntityFrameworkCore.Database.Command": "Warning"` — suppresses verbose SQL query logging. Set to `Information` in `appsettings.Development.json` for dev-only SQL visibility.
- `"Microsoft.AspNetCore.Mvc.Infrastructure": "Warning"` — suppresses the useless "No action descriptors found" startup message.

## Coding Conventions

- **No `Async` suffix** — don't name methods `RunAsync`, do `Run`. The `async` modifier on the method body is sufficient.
- **Models over tuples** — use a proper response class instead of `Task<(int, string, string)>`
- **No leading underscore** — name fields `inner`, `client`, `testKdbxPath`, not `_inner`, `_client`, `_testKdbxPath`

## Branching model

- **All work happens on the `dev` branch.** Commit directly to `dev`; do **not** create feature/topic branches that open a PR straight to `main`.
- `main` only ever changes via a merged **`dev` → `main`** PR. There is exactly one PR in flight at a time, from `dev` to `main`.
- So the loop is: commit on `dev` → push `dev` → open (or update) the `dev` → `main` PR → merge.

## PR Workflow

When creating or updating a PR from `dev` to `main`:

1. **Always run `git fetch origin main` first** — This is critical. The local `main` branch is often outdated and will show stale committed changes as part of the diff if not refreshed.
2. Compare `origin/main..dev` to identify only the actual new changes.
3. Check if a PR already exists (use `github_list_pull_requests`).
4. If none exists, create one with an accurate title and description summarizing the changes.
5. If one exists, update its title and description to reflect the actual current diff.
