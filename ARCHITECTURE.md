# Template.API Architecture

.NET 10 layered ASP.NET Core API template with a clean architecture.

## Projects

### Api

ASP.NET Core minimal API with endpoints organized by resource.
Each endpoint is a new cs file.
Each has minimal code to reach the application service.

There are test endpoints for test purposes.

All the exception mapping from Application happens here in ExceptionHandlerExtensions.cs.
Exception to ProblemDetails.

Antiforgery disabled but should be enabled in prod app.

...

### ApiClient

Client for the Api itself. A nuget can be created from this project. But mainly used for testing.

...

### Application

All the core business logic goes here. Domain is anemic.

...

### Core

Builder pattern and helpers that can be used in all projects.

...

### CrossCutting

Shared concerns: logging and settings mainly.

...

### Domain

Anemic domain entities and validators.

...

### Persistence

EF Core DbContext, configurations, and data access.

...

### Persistence.Migrations

Console app dbup-based SQL migration runner.

I prefer raw sql rather than efcore migrations with its own language (or even with raw sql). I think dbup is better.

...

## Tests

### Core.Testing

Shared test utilities: builders and validators.

...

### UnitTests

Isolated unit tests with no mocking. If you have to mock, probably, you needed an integration test.

...

### IntegrationTests

Mostly real API env tests. WebApplicationFactory + Testcontainers.MsSql.

This should be enough for testing almost everything.

...

### FunctionalTests

Tests a running API, local or deployed.

To be run after deployment to validate API works properly.

Several strategies to this:
    - Don't do functional tests: integration tests are enough, doesn't require a running API and if the API has UI, probably better to test the whole of the stack together via playwright or similar as an E2E.
    - Do only GETS: Tests for GET operations and validate API returns properly.
    - Test all: This would require some internal implementation if you are going to make changes to data in QA or PROD, or only run these tests in DEV env and in those more important envs just do GETS.

This repo does only GETS.
...