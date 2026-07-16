# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

> Project-wide principles — TDD cycle, Tidy First, commit discipline, commit-message
> format, and the "Never" list — live in the root `CLAUDE.md`. This file covers
> **backend-specific** guidance only.

## Workflow — Test-Driven Development

This project is developed with **TDD**. For **every new feature**, BEFORE writing any
implementation code:

1. **State the test plan first.** Enumerate the cases you will cover — happy path, edge
   cases, exclusions, aggregation/averaging, and error handling — and let the author
   confirm or adjust before implementation starts.
2. **Then implement**, writing the tests first.

A good test arranges realistic, varied data with builders, covers the happy path **and**
edge cases (empty/zero values, exclusions, error handling), and asserts precise expected
values rather than merely checking that a result exists.

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
# COMPOSE_PROFILES=local (set in .env) starts the bundled reverse-proxy,
# adminer, portainer and whoami. docker-compose.override.yaml (a copy of the Staging
# override) supplies the api/web build, so a bare `up` works locally.
docker compose up -d
```

> Production runs `docker-compose.yaml -f docker-compose.Production.yaml` with
> `COMPOSE_PROFILES` empty: the bundled infra (Traefik, …) stays down because the
> homelab provides ingress (gateway Traefik). Logs go to stdout → Loki/Grafana. See the
> deploy job in `.github/workflows/ci-cd.yml` (self-hosted runner on `lxc-holefeeder`).

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

Each Application feature folder contains `Commands/` and `Queries/` subdirectories following a CQRS pattern, plus a mapper (`*Mapper.cs`). Prefer keeping a feature's controller/use-cases/repositories together in its slice rather than grouping by technical role (a `Controllers/` folder, a `Services/` folder, etc.).

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
| Logging | Serilog → stdout (compact JSON; Loki/Grafana in prod) |
| Background jobs | Hangfire (PostgreSQL storage) |
| Frontend | Angular (pnpm) inside Blazor Server |
| Unit testing | xUnit + FluentAssertions (no mocks — NSubstitute is legacy, being phased out) |
| BDD/Functional tests | SpecFlow + Respawn + WireMock.Net |
| Coverage | Coverlet (Cobertura) + Stryker mutation testing |

### Code Quality Enforcement

- `TreatWarningsAsErrors=true` — all compiler warnings are errors
- Roslyn analyzers + StyleCop run in CI (`AnalysisMode=All`)
- Nullable reference types enabled everywhere
- Central package version management via `Directory.Packages.props`
- `tests/Directory.Build.props` enables Coverlet coverage output to `/coverage/`

### Local Infrastructure (docker-compose.yaml)

The local dev stack (profile `local`) includes: **Traefik** (reverse proxy with TLS), **PostgreSQL**, **PowerSync**, **Portainer**, **Adminer**. Logs go to stdout (`docker compose logs`). Configure via `.env` (copy from `.env.template`).

## Backend Conventions

### .NET

- Favor idiomatic .NET patterns.
- Limit the use of nulls; favor result/return values over exceptions (FluentResults).
- Do not add a library that has not been vetted — make a suggestion when appropriate.

### Testing — no mocks

- **Do not use mocks. Do not install a mocking library.** Favor James Shore's "testing
  without mocks" / Nullables approach: https://www.jamesshore.com/v2/projects/nullables/testing-without-mocks
- Sociable tests are preferred; narrow-focus integration tests are also welcome.
- Test against a real test database.
- Use FluentAssertions as much as possible. Multiple assertions for the same concept are
  welcome; use assertion scopes where appropriate.

### Strongly typed primitives

Wrap primitive IDs in typed wrappers so a `UserId` can only be used as a `UserId`:

```csharp
public record UserId(int Value);
```

Use `.Value` to access the underlying primitive when needed.

### SQL safety

- Never build SQL using string interpolation or concatenation — parameters only.

### Code style

- No code comments unless absolutely necessary.
- Use long, descriptive names for private methods and explicit, descriptive names for test methods.

### Resilience

This project showcases resiliency patterns (timeouts, circuit breakers, retries). Favor
standard .NET practices for resilience and the built-in time abstractions where appropriate.

### Developer experience

Keep local setup easy — ideally one command after cloning yields a working environment.
Docker is the container of choice; minimize manual setup steps.
