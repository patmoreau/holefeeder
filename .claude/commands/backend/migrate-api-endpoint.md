---
description: Move a legacy Holefeeder API endpoint into Holefeeder.Service's Pokeball layout, keeping its HTTP contract and the legacy tests green
---

<!-- Generated from docs/contributing/tasks/backend/migrate-api-endpoint.md by scripts/sync-agent-files.sh. Do not edit. -->

# Migrate a legacy endpoint to Holefeeder.Service

> **Audience:** contributors *working on* this repository. Not a published document — see `AGENTS.md` at the repository root.

A migration moves one endpoint from a legacy slice
(`backend/src/Holefeeder.Application/Features/<Feature>/{Commands,Queries}/<UseCase>.cs`)
into the feature/ring layout of `Holefeeder.Service`
(`docs/contributing/architecture-principles.md`, *Target architecture* in
`backend/AGENTS.md`). The first one done this way is the store items search:
`StoreItems/` in `Holefeeder.Service`, replacing the route in
`Holefeeder.Application/Features/StoreItems/Queries/GetStoreItems.cs`.

## Rules

1. **The HTTP contract does not change.** Same method and route, same query or body
   parameters, same status codes, same response body and headers (`X-Total-Count`),
   same problem details for errors. Clients cannot tell the endpoint moved.
2. **The legacy tests are the specification.** `Holefeeder.FunctionalTests` scenarios
   for the endpoint are trusted and stay **unchanged**. When one fails, the new
   endpoint is wrong: fix the endpoint, never the scenario. If the contract really
   must change, stop and ask; that is a separate change.
3. **Remove the legacy endpoint, not the request.** Delete the route (`ICarterModule`,
   `AddRoutes`, `MapGet`/`MapPost`) and the handler. Keep `Request`, `Response`,
   `Validator` and anything else the legacy tests or other legacy code still
   reference: the scenarios build their requests from them (`new GetStoreItems.Request(...)`).
   The class keeps only those nested types.
4. **Both suites pass.** The legacy scenarios and the new component tests in
   `Holefeeder.ServiceTests` must both be green before committing.

## Steps

1. **Read the legacy slice and its tests.** The slice file, its scenario in
   `tests/Holefeeder.FunctionalTests/Features/<Feature>/Scenario<UseCase>.cs`, the
   `Attempt...` cases in `Features/Authorization/` (authorized, forbidden,
   unauthorized), and the Refit signature in `Infrastructure/IUser.cs`. Write down the
   contract from rule 1 before writing any code.
2. **Build the feature the new way, test first.** In
   `src/Holefeeder.Service/<Feature>/`: `Core/<UseCase>/` (use case, value objects,
   port), `Persistence/` (Dapper and hand-written SQL), `Api/<UseCase>/` (endpoint,
   raw request, mapping). Unit tests use the in-memory port (`<Feature>InMemory` in the
   feature's `TestKit`), persistence tests use `DatabaseForTest`. Errors follow
   `ErrorConventions`; database failures are `UnexpectedError`, mapped to 500 by
   `ApiResult`.
3. **Mirror the scenario as component tests.** `<UseCase>Component` in
   `tests/Holefeeder.ServiceTests/<Feature>/Api/<UseCase>/` covers every case the
   legacy scenario covers, plus the cases the new code adds (an unregistered user, an
   unsupported filter). Unauthorized and forbidden calls stay covered by the legacy
   `Features/Authorization/` scenarios. `SearchStoreItemsComponent` against
   `ScenarioGetStoreItems` is the reference.
4. **Wire the feature.** `<Feature>.cs` at the feature root exposes
   `Add<Feature>Services` and `Map<Feature>Endpoints`; call both from `Program.cs`.
5. **Remove the legacy route in the same commit** that maps the new one (rule 3). Two
   endpoints on one route fail with an ambiguous match, and `main` must stay
   deployable.
6. **Verify** (from `backend/`):

   ```bash
   dotnet test tests/Holefeeder.ServiceTests/Holefeeder.ServiceTests.csproj
   dotnet test tests/Holefeeder.FunctionalTests/Holefeeder.FunctionalTests.csproj
   dotnet format DrifterApps.Holefeeder.slnx --verify-no-changes --severity error
   ```

   The service tests include `ArchitectureRules` and `ErrorConventions`.

## Pitfalls

- **Validation codes.** Legacy validators key errors by field; the new endpoint keys
  them by kebab-case error code. A scenario that checks only the status and the
  "One or more validation errors occurred." title passes either way; one that checks
  keys does not, and rule 2 applies.
- **Do not delete what the request needs.** Removing the legacy route can leave its
  validator or mapper unreferenced by production code; keep them while a legacy test
  uses them, and let the compiler tell you what is still used.
