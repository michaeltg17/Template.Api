[![CI](https://github.com/michaeltg17/Template.Api/actions/workflows/ci.yml/badge.svg)](https://github.com/michaeltg17/Template.Api/actions/workflows/ci.yml)
# Template.Api

.NET 10, ASP.NET Core Api + Tests template of my recommended architecture for a successful, dev efficient and scalable solution. 

Full description of the architecture there: [ARCH.md](https://github.com/michaeltg17/Template.Api/blob/main/ARCH.md)

Built with the help of local AI using https://github.com/michaeltg17/best-model-dual-3090 and [OpenCode](https://github.com/anomalyco/opencode).

## Tech stack
API:
- ASP.NET Core Minimal API
- OpenAPI
- Feature-based architecture
- ProblemDetails
- N-Layer Architecture
- Anemic Domain Model
- Entity Framework Core
- PostgreSQL

Tests:
- Unit, integration and functional tests
- xUnit
- WireMock
- AwesomeAssertions
- Coverlet + ReportGenerator

Branching:
- dev branch for continuous fast development which is then merged to main for stable versions.

CI:
- In docker with ci.sh that runs in GitHub Actions and can also be run locally.
- Does Build + Tests + Coverage. If main, it also pushes `template-api` and `template-db-migrations` Docker images to ghcr + Tag + Release.

CD:
- Done in [Template.Deployment](https://github.com/michaeltg17/Template.Deployment)

UI:
- Done in [Template.React](https://github.com/michaeltg17/Template.React)