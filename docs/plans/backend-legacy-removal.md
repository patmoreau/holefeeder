# Remove the legacy backend projects

Status: **steps 1–5 done**, step 6 next, recorded 2026-11-12. Finishes step 7 of
[backend-service.md](backend-service.md).

## Context

Every route is served by `Holefeeder.Service`, and the dead legacy code is gone
(`270d58cd`, `df331429`, `c037ef50`, `9347acc9`, `9d5cca05`). What is left of
`Holefeeder.Application`, `.Domain` and `.Infrastructure` has two kinds of users.

**The host, in production:**

| Legacy code | Used for |
|---|---|
| ~~`Application/Authorization/Configuration`~~ | Moved in step 3 |
| ~~`Infrastructure/SeedWork/BudgetingConnectionStringBuilder`~~ | Replaced in step 4 |
| ~~`Infrastructure/Extensions/ServiceCollectionExtensions`~~ | `AddInfrastructure` removed in step 4, `AddHangfireServices` moved in step 5 |
| `Infrastructure/Extensions/WebApplicationExtensions` | `MigrateDb`: DbUp over the scripts embedded in `Holefeeder.Infrastructure`, 3 tries 10 s apart; also called by `DatabaseForTest` |
| `Infrastructure/Scripts/*.sql` | The schema, as DbUp scripts |
| `Application/Extensions/LoggerExtensions` | The three migration log messages `MigrateDb` writes |

**The legacy tests:** `Holefeeder.FunctionalTests` (115 scenarios) and
`Holefeeder.Tests.Common` build on everything else: the `Request`/`Response` types in
`Application/Features` and `UseCases/PowerSync`, `CommandStatus`, the MyData DTOs,
the JSON converters, the EF Core `BudgetingContext` they seed the database with, and
the Domain entities and value objects.

Every functional scenario except authorization has a component twin in
`Holefeeder.ServiceTests`, written when its route moved. The 53 authorization
scenarios (`Features/Authorization/`: authorized, 401 unauthenticated, 403 without
scope, one per route) have none. They are the only coverage the functional tests
still add.

## Steps

Each step leaves `main` deployable and every suite green.

1. **Authorization component test.** In `Holefeeder.ServiceTests/Shared/Api/`, one
   data-driven component that reads the routes from the running service's
   `EndpointDataSource`, so a new endpoint is covered without editing the test. For each
   route that requires authorization: no token gives 401, a token without the needed
   scope gives 403, and a token with it gives neither. List the routes that allow
   anonymous access (`/healthz`, OpenAPI, Scalar, Hangfire dashboard) explicitly, and
   fail when a route is neither authorized nor on that list. `ServerForTest` needs a
   client that sends no token and one whose token carries a chosen scope. Test-only
   commit.
   Done: `Shared/Api/AuthorizationComponent` covers all 43 API routes (the functional
   scenarios covered 17). The route's scope is read from its own policy, so the test
   catches a missing or bypassed policy, not a route given the wrong one.
2. **Delete `Holefeeder.FunctionalTests` and `Holefeeder.Tests.Common`.** Structural.
   Remove the projects, both `.slnx` entries, the `api-functional-tests` CI matrix entry,
   the `InternalsVisibleTo` entries in `src/Directory.Build.props`, and their mention in
   `backend/AGENTS.md` and the endpoint playbooks (rerun `scripts/sync-agent-files.sh`).
   The packages they alone use go too: `DrifterApps.Seeds.Testing`, `Respawn`,
   `WireMock.Net`, `Nito.AsyncEx` and `Nito.AsyncEx.Coordination` from
   `tests/Directory.Packages.props`, and `Refit` from `Directory.Packages.props`.
   Check each against `Holefeeder.ServiceTests`, which still references `Bogus` and
   `NSubstitute`, first.
   Done. The `migrate-api-endpoint` playbook went with them: it took the functional
   scenarios as its specification and no legacy route is left to migrate. So did the
   `SECURITY_TESTS` environment checks in `Program.cs` and the CI step that trusted the
   development certificate for the functional tests' identity server.
3. **Move the auth constants.** Structural. `Authorization/Configuration` becomes a
   `Support/Api` type, and `AddSecurity` uses it. The endpoints keep their own
   `ReadUserPolicy`/`WriteUserPolicy` constants.
   Done differently: `AddSecurity` is their only user, so they became private constants
   there instead of a `Support/Api` type. The unused `Roles.Admin` was dropped.
4. **Replace the connection string builder.** Structural. Register the `NpgsqlDataSource`
   straight from `ConnectionStrings:BudgetingConnectionString` (the key does not change,
   so neither Staging nor the production Quadlet units do). Hangfire storage and the
   migration runner take their connection string from that data source. `ServerForTest`
   keeps overriding the configuration key and stops replacing the builder.
   Done differently: `NpgsqlDataSource.ConnectionString` drops the password, so Hangfire
   and DbUp cannot take theirs from the data source. The host's
   `Extensions/DatabaseConnection` reads the key once per caller instead, and passes it
   to the data source, `AddHangfireServices` and `MigrateDb`. The dead EF Core
   `DatabaseFacade.MigrateDb` overload went too.
5. **Move Hangfire setup.** Structural. `AddHangfireServices` joins `UseHangfire` in
   `Extensions/` of the host, unchanged: same storage, serializer settings and one
   worker. Jobs already enqueued keep deserializing, because `ImportDataPayload`'s type
   name does not change.
   Done: it sits in the host's `Extensions/ServiceCollectionExtensions`, and the Service
   references `Hangfire.AspNetCore` and `Hangfire.PostgreSql` directly. The `Hangfire`
   meta-package went with Infrastructure's references; it only added the unused SQL
   Server storage, and the resolved Hangfire versions stay the same.
6. **Move the schema migrations.** Structural. The DbUp runner, its log messages and the
   `.sql` files move into `Holefeeder.Service`; `DatabaseForTest` calls the new runner.
   The location is a decision to settle first. It holds no feature, so the proposal is
   `Support/Persistence/Schema/`, with `Support` covering what every feature relies on.
   DbUp's journal (`schemaversions`) names each script after its embedded resource,
   `Holefeeder.Infrastructure.Scripts.001-CreateAccountsTable.sql`. Keep those names with
   a `LogicalName` on each `EmbeddedResource` (`Holefeeder.Infrastructure.Scripts.%(Filename)%(Extension)`)
   so no script runs twice. Renaming the journal rows works too, but adds a script that
   must run before DbUp reads its own journal. Prove the move with a persistence test:
   on a database migrated by the old runner, the new one applies nothing. The
   `Decisions` entry on schema migrations in `backend-service.md` and the
   `add-api-endpoint` playbook then point at the new folder.
7. **Delete `Holefeeder.Application`, `.Domain` and `.Infrastructure`.** Structural. Drop
   the project references from `Holefeeder.Service.csproj`, the projects from both
   `.slnx` files, their `COPY` lines from `src/Holefeeder.Service/Dockerfile`, and the
   packages only they use: EF Core and `Npgsql.EntityFrameworkCore.PostgreSQL`,
   `Ardalis.SmartEnum*`, `Dapper.FluentMap`, `Dapper.SqlBuilder`, `Dapper.Transaction`,
   `System.Data.SqlClient`, `DrifterApps.Seeds.Application` and `DrifterApps.Seeds.Domain`. `ArchitectureRules.LegacyNamespaces` can then go too, or
   stay as a guard against the packages coming back. Build the Docker image and deploy
   to Staging before calling it done.
8. **Docs.** Remove *Legacy architecture* from `backend/AGENTS.md` and make *Target
   architecture* the only one; close step 7 in `backend-service.md`.

## Decisions to make before step 6

- **Where the schema lives** in `Holefeeder.Service` (proposed `Support/Persistence/Schema/`).
- **Journal names**: keep the `Holefeeder.Infrastructure.Scripts.` resource names
  (proposed) or rename the `schemaversions` rows.
