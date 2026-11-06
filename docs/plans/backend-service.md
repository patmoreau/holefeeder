# Consolidate the backend into Holefeeder.Service

Status: **steps 1–4 done**, step 5 next, recorded 2026-10-31.

## Context

Most of the backend was written before agent-assisted work and is now **legacy**: four
class libraries (`Holefeeder.Api`, `.Application`, `.Domain`, `.Infrastructure`), EF
Core alongside Dapper, and the `DrifterApps.Seeds.*` packages from the
`holefeeder-seed.work` repository.

The target is described in `backend/AGENTS.md` under *Target architecture*:

- one project, `Holefeeder.Service`, with one test project, `Holefeeder.ServiceTests`
- the feature/ring layout from `docs/contributing/architecture-principles.md`, in
  PascalCase
- Dapper with hand-written SQL, no EF Core
- seed.work code copied in only when a feature needs it; `DrifterApps.Seeds.FluentResult`
  and `DrifterApps.Seeds.FluentScenario` (with their `.FluentAssertions` companions) kept
  as packages

Legacy code is **not** refactored until one complete feature built the new way exists and
is approved. Every step leaves `main` deployable.

## Steps

1. ✅ **Document the target.** `backend/AGENTS.md`, this plan, and a legacy notice on the
   `add-api-endpoint` playbook. Done in `230c6866`.
2. ✅ **Rename `Holefeeder.Api` to `Holefeeder.Service`.** Structural commit: folder,
   `.csproj`, `Holefeeder.Api` namespaces, both `.slnx` files, test project references,
   `Dockerfile` path in `ci-cd.yml`, `codeql-analysis.yml`, `docker-compose.override.yaml`,
   `docker-compose-tests.yaml`, `.vscode/launch.json`, and the docs that name it. Keep the
   published image name and the `api` matrix target and compose service unchanged, so the
   production Quadlet units keep pulling the same image. Deploy to Staging to confirm.
   Done in `cd0f6a38` and deployed to Staging. The Scalar Auth0 `ClientId` and the
   Serilog `ApplicationName` also kept their `Holefeeder.Api` values.
3. ✅ **Add `Holefeeder.Service.Tests`.** One sociable smoke test written with
   FluentScenario against the running service and a real database. Copy from
   `DrifterApps.Seeds.Testing` only what that test needs. Done in `86ca70e5`: the
   scenario asks a freshly started service for `/healthz` against an empty database,
   and runs in CI as `api-service-tests`.
4. ✅ **Enforce the rules with architecture tests.** In `Holefeeder.Service.Tests`: code
   under `Features/` and `Shared/` does not use legacy namespaces, rings depend inward
   only, features do not reference each other. Done in `cdc596a5` with ArchUnitNET
   (`Architecture/ArchitectureRules.cs`), which also checks that `Shared` does not
   depend on `Features`.
5. **Build the first feature the new way**, end to end — `Api`, `Core`, `Persistence`,
   Dapper SQL, tests. Feature to be chosen. Ships hidden until complete.
6. **Review and settle the pattern.** Adjust `backend/AGENTS.md` to what the feature
   taught, and rewrite the `add-api-endpoint` playbook for the new layout.
7. **Migrate legacy features one at a time** (separate plan). Each move deletes the
   legacy code it replaces. When the last user is gone: drop EF Core, the remaining
   `DrifterApps.Seeds.*` packages, `Holefeeder.Domain`, `.Application`, `.Infrastructure`,
   `Holefeeder.UnitTests`, `Holefeeder.FunctionalTests` and `Holefeeder.Tests.Common`.
   Before `Holefeeder.Infrastructure` goes, move the DbUp runner and its scripts into
   `Holefeeder.Service`. DbUp's `schemaversions` journal records each script by its
   embedded resource name, which starts with the assembly name
   (`Holefeeder.Infrastructure.Scripts.001-CreateAccountsTable.sql`), so moving the
   files would rerun every script. The move must rename the journal rows in a script of
   its own, or keep the old names.

## Decisions

- **Schema migrations.** `app.MigrateDb()` in `Program.cs` runs DbUp at startup: it
  applies the scripts embedded in `Holefeeder.Infrastructure`, in name order, one
  transaction each, and skips those already in `schemaversions`. New and legacy code
  share one schema, so scripts for new tables also go in
  `Holefeeder.Infrastructure/Scripts/` (next number, plus an `EmbeddedResource` entry in
  the `.csproj`) until step 7 moves them. This is the one legacy location new work
  writes to.
- **Seed.work source.** Code is copied from the seed.work repository at tag `v1.0.179`,
  the version the legacy projects reference, so copied code matches what runs today.
  `Holefeeder.Service.Tests` holds the first copies: `PostgreDatabaseServer` and
  `LocalDatabaseServer`, trimmed to what the smoke test uses.
- **Test database.** `Holefeeder.Service.Tests` starts PostgreSQL with Testcontainers
  (`Testcontainers.PostgreSql`, approved as a direct reference) unless
  `LOCAL_CONNECTION_STRING` is set, which is how CI runs it against the
  `postgres-data-test` compose service (`api-service-tests` in the ci-cd matrix).
- **Architecture tests.** `TngTech.ArchUnitNET` and `TngTech.ArchUnitNET.xUnitV3` are
  approved as test-only references. NetArchTest was rejected: no release since 2021.
  ArchUnitNET reads IL, so a legacy type used only inside a method body still fails a
  rule. The rules match namespaces (`Holefeeder.Service.<Feature>.<Ring>`,
  `Holefeeder.Service.Shared.<Ring>`, `Holefeeder.Service.Support.<Ring>`), so folders
  and namespaces must stay in step. Features live at the project root, not under
  `Features/`: the rules treat any root namespace with a ring under it as a feature, and
  `Shared` and `Support` as cross-cutting modules every feature may use.
