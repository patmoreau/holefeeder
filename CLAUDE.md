# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

### Build
```bash
dotnet build DrifterApps.Holefeeder.sln
```

### Test
```bash
# All tests (excluding test helpers)
dotnet test --settings .runsettings --filter "FullyQualifiedName!~Holefeeder.Tests.Common"

# Single test project
dotnet test tests/Holefeeder.UnitTests/

# Single test by name filter
dotnet test --filter "FullyQualifiedName~MyTestClass"
```

### Lint
```bash
# C#
dotnet format --severity error --verbosity diagnostic

# TypeScript (Angular frontend)
cd src/Holefeeder.Web/ClientApp && pnpm lint
```

### Code Coverage
```bash
dotnet test --settings .runsettings --filter "FullyQualifiedName!~Holefeeder.Tests.Common"
dotnet reportgenerator -reports:coverage/coverage.cobertura.xml -targetdir:coverage -filefilters:-*.g.cs
```

### Docker (local dev)
```bash
docker compose -f docker-compose.yaml up -d
```

### Run tests in Docker (as CI does)
```bash
docker compose -f docker-compose-tests.yaml run api-unit-tests
docker compose -f docker-compose-tests.yaml run api-functional-tests
```

## Architecture

### Layer Structure

```
Domain → Application → Infrastructure → Api / Web
```

- **Domain** (`src/Holefeeder.Domain`): Aggregate roots, value objects, domain events. No dependencies on outer layers.
- **Application** (`src/Holefeeder.Application`): Use cases, CQRS queries/commands, application DTOs, authorization. References Domain only.
- **Infrastructure** (`src/Holefeeder.Infrastructure`): PostgreSQL via Dapper + EF Core, DbUp migrations, Hangfire background jobs. Implements interfaces from Application.
- **Api** (`src/Holefeeder.Api`): ASP.NET Core 10 minimal API using the **Carter** framework for route modules. Thin layer — delegates to Application.
- **Web** (`src/Holefeeder.Web`): Blazor Server hosting an Angular SPA (`ClientApp/` built with pnpm).

### Feature Organization

Both Domain and Application are organized into **vertical feature slices** by business domain:

- Application features: `Accounts`, `Categories`, `Dashboard`, `Enumerations`, `MyData`, `Periods`, `Statistics`, `StoreItems`, `Tags`, `Transactions`
- Domain features: `Accounts`, `Categories`, `StoreItem`, `Transactions`, `Users`

Each Application feature folder contains `Commands/` and `Queries/` subdirectories following a CQRS pattern, plus a mapper (`*Mapper.cs`).

### Key Patterns

- **Strongly typed IDs**: Entity identifiers are custom value objects (not raw `Guid`/`int`) — see `Domain/ValueObjects/`.
- **Result pattern**: FluentResults is used for error handling rather than exceptions in use cases.
- **Carter modules**: API routes are defined as Carter `ICarterModule` implementations per feature, not controller classes.
- **SmartEnum**: Enumerations are strongly typed via the SmartEnum pattern — see `Domain/Enumerations/`.
- **PowerSync**: Sync endpoint for offline-capable clients — `MyData` feature in Application/Infrastructure.

### Technology Stack

| Concern | Technology |
|---|---|
| Runtime | .NET 10 |
| API routing | Carter (Minimal APIs) |
| Database | PostgreSQL 18 |
| ORM | Dapper (queries) + EF Core (migrations support) |
| DB migrations | DbUp |
| Authentication | Auth0 via JWT + `Microsoft.Identity.Web` |
| Logging | Serilog → Seq |
| Background jobs | Hangfire (PostgreSQL storage) |
| Frontend | Angular (pnpm) inside Blazor Server |
| Unit testing | xUnit + NSubstitute + FluentAssertions |
| BDD/Functional tests | SpecFlow + Respawn + WireMock.Net |
| Coverage | Coverlet (Cobertura) + Stryker mutation testing |

### Code Quality Enforcement

- `TreatWarningsAsErrors=true` — all compiler warnings are errors
- Roslyn analyzers + StyleCop run in CI (`AnalysisMode=All`)
- Nullable reference types enabled everywhere
- Central package version management via `Directory.Packages.props`
- `tests/Directory.Build.props` enables Coverlet coverage output to `/coverage/`

### Local Infrastructure (docker-compose.yaml)

The local dev stack includes: **Traefik** (reverse proxy with TLS), **PostgreSQL**, **Seq** (log viewer at `seq.localhost`), **PowerSync**, **Portainer**, **Adminer**. Configure via `.env` (copy from `.env.template`).
