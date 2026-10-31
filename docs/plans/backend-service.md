# Consolidate the backend into Holefeeder.Service

Status: **step 1 done** (guidance documented), recorded 2026-10-31.

## Context

Most of the backend was written before agent-assisted work and is now **legacy**: four
class libraries (`Holefeeder.Api`, `.Application`, `.Domain`, `.Infrastructure`), EF
Core alongside Dapper, and the `DrifterApps.Seeds.*` packages from the
`holefeeder-seed.work` repository.

The target is described in `backend/AGENTS.md` under *Target architecture*:

- one project, `Holefeeder.Service`, with one test project, `Holefeeder.Service.Tests`
- the feature/ring layout from `docs/contributing/architecture-principles.md`, in
  PascalCase
- Dapper with hand-written SQL, no EF Core
- seed.work code copied in only when a feature needs it; `DrifterApps.Seeds.FluentResult`
  and `DrifterApps.Seeds.FluentScenario` (with their `.FluentAssertions` companions) kept
  as packages

Legacy code is **not** refactored until one complete feature built the new way exists and
is approved. Every step leaves `main` deployable.

## Steps

1. **Document the target.** `backend/AGENTS.md`, this plan, and a legacy notice on the
   `add-api-endpoint` playbook.
2. **Rename `Holefeeder.Api` to `Holefeeder.Service`.** Structural commit: folder,
   `.csproj`, `Holefeeder.Api` namespaces, both `.slnx` files, test project references,
   `Dockerfile` path in `ci-cd.yml`, `codeql-analysis.yml`, `docker-compose.override.yaml`,
   `docker-compose-tests.yaml`, `.vscode/launch.json`, and the docs that name it. Keep the
   published image name and the `api` matrix target and compose service unchanged, so the
   production Quadlet units keep pulling the same image. Deploy to Staging to confirm.
3. **Add `Holefeeder.Service.Tests`.** One sociable smoke test written with
   FluentScenario against the running service and a real database. Copy from
   `DrifterApps.Seeds.Testing` only what that test needs.
4. **Enforce the rules with architecture tests.** In `Holefeeder.Service.Tests`: code
   under `Features/` and `Shared/` does not use legacy namespaces, rings depend inward
   only, features do not reference each other. Choose between a library (needs approval)
   and plain reflection, then replace `<arch test command>` in
   `docs/contributing/architecture-principles.md`.
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

## Open questions

- **Seed.work source**: copy from the `holefeeder-seed.work` repository at the version
  currently referenced (1.0.179), so copied code matches what runs today.
