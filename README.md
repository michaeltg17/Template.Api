[![CI](https://github.com/michaeltg17/Template.Api/actions/workflows/ci.yml/badge.svg)](https://github.com/michaeltg17/Template.Api/actions/workflows/ci.yml)
# Template.Api

.NET 10, ASP.NET Core Api + Tests template of my recommended architecture for a successful, dev efficient and scalable solution. 

Full description of the architecture there: [ARCHITECTURE.md](https://github.com/michaeltg17/Template.Api/blob/main/ARCHITECTURE.md)

Built with the help of local AI using https://github.com/michaeltg17/best-model-dual-3090 and [OpenCode](https://github.com/anomalyco/opencode).

## Tech stack
API:
- ASP.NET Core
- OpenAPI
- ProblemDetails
- N-Layer Architecture
- Anemic Domain Model
- Services
- Entity Framework Core
- PostgreSQL

Tests:
- Unit, integration and functional tests
- xUnit
- WireMock
- AwesomeAssertions
- Coverlet + ReportGenerator

CI/CD:
- CI in docker with ci.sh that runs in GitHub Actions and can also be run locally.
- CI does Build + Tests + Coverage. If main, it also does Docker image push to ghcr, creates tag and release.
- dev branch for continuous fast development which is then merged to main for stable versions.
