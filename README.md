[![CI](https://github.com/michaeltg17/Template.Api/actions/workflows/ci.yml/badge.svg)](https://github.com/michaeltg17/Template.Api/actions/workflows/ci.yml)
# Template.Api

.NET 10, ASP.NET Core Api + Tests template of my recommended architecture for a successful, dev efficient and scalable solution. 

Full description of the architecture there: [ARCHITECTURE.md](https://github.com/michaeltg17/Template.Api/blob/main/ARCHITECTURE.md)

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
- Moq
- AwesomeAssertions
- Coverlet + ReportGenerator

CI/CD:
- CI in docker with ci.sh that runs in GitHub Actions and can also be run locally.
- CI does Build + Tests + Coverage + if main, Docker image push to ghcr.
- dev branch for continuous fast development which is then merged to main for stable versions.
