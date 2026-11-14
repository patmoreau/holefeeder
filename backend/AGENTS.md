# Holefeeder — Backend Instructions

This file provides guidance when working with code in this repository.

> Project-wide principles — TDD cycle, Tidy First, commit discipline, commit-message
> format, and the "Never" list — live in the root `../AGENTS.md` and the process docs it
> includes (`../docs/contributing/`). This file covers **backend-specific** guidance only.

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

## Architecture — Holefeeder.Service

The backend is one ASP.NET Core 11 minimal API. The store items search
(`src/Holefeeder.Service/StoreItems/`) is the reference implementation. The legacy
layered projects (`Holefeeder.Domain`, `.Application`, `.Infrastructure`) it replaced are
gone; how they were retired is in `../docs/plans/backend-service.md` and
`../docs/plans/backend-legacy-removal.md`.

The `/backend:add-api-endpoint` playbook carries the procedure for a new endpoint
(`../docs/contributing/tasks/backend/`).

- **One project.** `src/Holefeeder.Service` (the API host, formerly `Holefeeder.Api`) and
  `tests/Holefeeder.ServiceTests`. No new class libraries.
- **Feature/ring layout** from `../docs/contributing/architecture-principles.md`, in
  PascalCase so folders match namespaces. Features sit directly under the project root,
  next to the host folders (`Extensions/`, `ErrorHandling/`):
  ```
  src/Holefeeder.Service/
    <FeaturePlural>/
      Api/<UseCase>/               endpoint, raw request, response
      Core/                        value objects used by several of the feature's use cases
      Core/<UseCase>/              use case, request, port, returned types
      Jobs/<JobName>/
      Persistence/                 <FeaturePlural>InDatabase, rows, SQL clause builders
      <FeaturePlural>.cs           service registration and endpoint mapping
    Shared/<Ring>/
    Support/<Ring>/

  tests/Holefeeder.ServiceTests/
    <FeaturePlural>/               same folders as the source, plus TestKit/
    Shared/                        ArchitectureRules, ErrorConventions, shared code's tests
    TestKit/                       ServerForTest, DatabaseForTest, test markers
  ```
  A root folder counts as a feature as soon as it has a ring folder. `Jobs` is
  implemented with Hangfire: Hangfire is the tool, the ring is named for what it does.
  The same goes for every ring — name it for its role, not its library.
- **Cross-cutting modules.** `Shared` and `Support` are not features. Any feature may
  depend on them; they never depend on a feature. `Support` holds the request's user
  (`User`, `IdentityObjectId`, `UserContextAccessor`), the middleware that resolves
  it, and the schema migrations (`Support/Persistence/Schema/`).
- **No legacy dependencies.** `ArchitectureRules` fails if code in features, `Shared/` or
  `Support/` uses the namespaces of the deleted legacy projects or of the
  `DrifterApps.Seeds.Application`/`.Domain`/`.Infrastructure`/`.Testing` packages.
- **Dapper with hand-written SQL.** No EF Core in new code. The persistence ring owns
  the SQL and its row models and maps them to core types. Schema changes are DbUp
  scripts in `src/Holefeeder.Service/Support/Persistence/Schema/Scripts/`, numbered;
  they keep their `Holefeeder.Infrastructure.Scripts.` resource names (see *Schema
  migrations* in `../docs/plans/backend-service.md`).
- **One upsert per port.** Writes go through a single `SaveAsync` (`INSERT ... ON
  CONFLICT (id) DO UPDATE` guarded on the owner), never separate create and modify
  methods; use cases load, change, then save.
- **Always-valid core types.** Value objects have a private constructor, a `Create`
  that validates and returns `Result<T>`, and a `Valid` used only by test values and
  database reads. Each also parses its own text with `Create(string?)`.
- **Raw requests take strings.** Route values are `string`, query values `string?`
  (null means the default), and a body is a typed `<UseCase>Body` record of `string?`
  named after the JSON, held as `[FromBody]`; the endpoint binds the raw request with
  `[AsParameters]`.
- **Results end to end.** New code never throws for an expected failure. Each error is
  a static member of an `*Errors` class with a kebab-case code constant and a
  one-sentence message; `ErrorConventions` checks codes are kebab-case and unique.
  Persistence catches database exceptions and returns an `UnexpectedError`.
  `Shared/Api/ApiResult` alone turns errors into HTTP: validation aggregates are 400
  validation problems, `UnexpectedError` is 500, `NotFoundError` is 404, any other
  error is 400.
- **Tests without mocks.** Core is tested against an in-memory port
  (`<FeaturePlural>InMemory` in the feature's `TestKit/`), persistence against
  PostgreSQL from `DatabaseForTest`, and each endpoint by a component test over HTTP
  with `ServerForTest` and FluentScenario. Builders are named `A<Thing>()`.
- **Seed libraries.** Keep using `DrifterApps.Seeds.FluentResult` and
  `DrifterApps.Seeds.FluentScenario`, with their `.AwesomeAssertions` companions, as
  packages. Anything else from the `holefeeder-seed.work` repository is copied into the
  core, ring or test project that needs it, only when it is needed, and owned from then
  on.

## Commands

### Build
```bash
dotnet build DrifterApps.Holefeeder.slnx
```

### Test
```bash
# Service tests (Holefeeder.Service, real PostgreSQL via Testcontainers)
dotnet test tests/Holefeeder.ServiceTests/Holefeeder.ServiceTests.csproj

# Architecture rules only (ArchUnitNET, no database needed)
dotnet test tests/Holefeeder.ServiceTests/Holefeeder.ServiceTests.csproj -- --filter-class "*ArchitectureRules"

# Single test class (VSTest's --filter is not available; the test platform
# takes its own arguments after --)
dotnet test tests/Holefeeder.ServiceTests/Holefeeder.ServiceTests.csproj -- --filter-class "*MyTestClass"
```

> `--settings .runsettings` reports **"Zero tests ran"** — the projects run on
> Microsoft.Testing.Platform, which ignores `.runsettings`. Pass the project path
> instead.

> **Podman (rootless, macOS):** the service tests start Testcontainers, and Ryuk
> fails with `making volume mountpoint … operation not supported`. Run them with
> `TESTCONTAINERS_RYUK_DISABLED=true`, and make sure `DOCKER_HOST` points at the
> podman machine socket (see the Docker section below).

### Lint
```bash
# C#
dotnet format --severity error --verbosity diagnostic
```

### Code Coverage
```bash
dotnet test --project tests/Holefeeder.ServiceTests/Holefeeder.ServiceTests.csproj \
  --coverage --coverage-output-format cobertura \
  --coverage-output coverage.cobertura.xml --results-directory coverage
dotnet reportgenerator -reports:coverage/coverage.cobertura.xml -targetdir:coverage '-filefilters:-*.g.cs'
```

### Docker (local dev)
```bash
# COMPOSE_PROFILES=local (set in .env) starts the bundled reverse-proxy,
# adminer, portainer and whoami. docker-compose.override.yaml (a copy of the Staging
# override) supplies the api/web build, so a bare `up` works locally.
docker compose up -d
```

> **Podman (rootless, macOS):** the CLI is Docker-compatible, so `docker compose`
> works unchanged once the tooling points at the podman machine socket (the
> `.zshrc` snippet exports this automatically):
> ```bash
> export DOCKER_HOST="unix://$(podman machine inspect --format '{{.ConnectionInfo.PodmanSocket.Path}}')"
> ```
> The core stack (api/web/postgres/powersync) needs nothing more. The bundled
> `local`-profile reverse-proxy additionally needs, one-time per machine:
> ```bash
> # Angular prod build OOMs on the 2 GiB default machine — give it headroom:
> podman machine stop && podman machine set --memory 8192 --cpus 5 && podman machine start
> # Rootless podman can't bind :80/:443 without this (inside the VM):
> podman machine ssh 'echo net.ipv4.ip_unprivileged_port_start=80 | sudo tee /etc/sysctl.d/99-unprivileged-ports.conf && sudo sysctl -p /etc/sysctl.d/99-unprivileged-ports.conf'
> ```
> and in `.env`, `DOCKER_SOCK` must point at the **in-VM** rootless socket (there is
> no `/var/run/docker.sock` in the podman machine): `/run/user/<uid>/podman/podman.sock`,
> where `<uid>` = `podman machine ssh id -u` (typically your macOS uid, e.g. 501).
> The reverse-proxy/portainer services carry `security_opt: label:disable` so the
> SELinux-enforcing machine lets Traefik read that socket.

> Production does **not** use Docker/Podman Compose. The api/web containers run as
> rootful Podman **Quadlet units** (provisioned by the my-homelab github-runner role)
> on `lxc-holefeeder`; the homelab gateway Traefik provides ingress and logs go to
> stdout → Loki/Grafana. The deploy job in `.github/workflows/ci-cd.yml` only rolls
> images: `sudo podman auto-update` pulls the newest `:latest` (published by the
> `publish-docker-images` job) and restarts changed units, gated on manual approval.

### Tests in CI
The `tests` job in `.github/workflows/ci-cd.yml` runs `dotnet test` for each test
project directly on the runner, not in Docker, so Testcontainers can reach the
runner's Docker.
The Dockerfile only builds the published image.

## Platform

### Technology Stack

| Concern | Technology |
|---|---|
| Runtime | .NET 11 |
| API routing | Minimal APIs |
| Database | PostgreSQL 18 |
| Data access | Dapper with hand-written SQL; no EF Core |
| DB migrations | DbUp |
| Authentication | Auth0 via JWT + `Microsoft.Identity.Web` |
| Logging | Serilog → stdout (compact JSON; Loki/Grafana in prod) |
| Background jobs | Hangfire (PostgreSQL storage) |
| Frontend | React + MUI in `frontend/apps/holefeeder-web`, served by nginx from its own image |
| Testing | xUnit v3 + AwesomeAssertions + FluentScenario, Testcontainers PostgreSQL; no mocking library |
| Coverage | `dotnet test --coverage --coverage-output-format cobertura` (Microsoft.Testing.Platform), as CI runs it |

### Code Quality Enforcement

- `TreatWarningsAsErrors=true` — all compiler warnings are errors
- Roslyn analyzers run on every build (`AnalysisMode=All`)
- Nullable reference types enabled everywhere
- Central package version management via `Directory.Packages.props`

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
- Use AwesomeAssertions as much as possible. Multiple assertions for the same concept are
  welcome; use assertion scopes where appropriate.

### Strongly typed primitives

Wrap primitives in typed value objects so a `Code` can only be used as a `Code`. In new
code they follow the always-valid pattern (*Architecture*); `Code` in
`src/Holefeeder.Service/StoreItems/Core/Code.cs` is the example. Use `.Value` to reach
the underlying primitive.

### SQL safety

- Never put a value into SQL through string interpolation or concatenation — parameters
  only. The one exception is clause fragments built from fixed values: a column name
  from a fixed map or an operator from an enum, as in
  `SearchStoreItemsRequestWhereClause` and `SearchStoreItemsRequestOrderByClause`.

### Code style

- No code comments unless absolutely necessary.
- Use long, descriptive names for private methods and explicit, descriptive names for test methods.

### Resilience

When code calls an external service, add timeouts, retries and circuit breakers with the
standard .NET resilience stack (`Microsoft.Extensions.Http.Resilience`) rather than
hand-rolled loops; like any new library, propose it before adding it. Inject
`TimeProvider` instead of reading `DateTime.Now`/`UtcNow` so time-dependent code stays
testable.

### Developer experience

Keep local setup easy — ideally one command after cloning yields a working environment.
Docker is the container of choice; minimize manual setup steps.
