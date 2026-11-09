---
description: Add a REST endpoint to Holefeeder.Service as a Pokeball feature (Core, Persistence, Api) with Result errors and tests, modeled on SearchStoreItems
---

<!-- Generated from docs/contributing/tasks/backend/add-api-endpoint.md by scripts/sync-agent-files.sh. Do not edit. -->

# Add a REST endpoint to the Holefeeder backend

> **Audience:** contributors *working on* this repository. Not a published document — see `AGENTS.md` at the repository root.

New endpoints live in `backend/src/Holefeeder.Service`, in the feature/ring layout of
`docs/contributing/architecture-principles.md`. Never add one to the legacy slices in
`Holefeeder.Application/Features/`. To move an existing legacy endpoint, follow
`migrate-api-endpoint.md` instead; it builds on this playbook.

**The reference is the store items search**, `GET api/v2/store-items`
(`src/Holefeeder.Service/StoreItems/`). When this playbook and that code disagree, the
code wins and this playbook is the bug to fix.

Read `backend/AGENTS.md` first. TDD is required: state the test plan before writing
implementation code, then one failing test at a time.

## 1. Decide the shape

| Question | Convention |
|---|---|
| Feature | A folder at the `Holefeeder.Service` root, plural PascalCase (`StoreItems`, `Accounts`). Not under `Features/` |
| Use case | Verb + noun in PascalCase: `SearchStoreItems` for a paged list, `GetStoreItem`, `OpenAccount` |
| Route | Collection read `api/v2/<feature-kebab>`; single read `api/v2/<feature-kebab>/{id:guid}`; command `api/v2/<feature-kebab>/<use-case-kebab>` |
| Policy | `"ReadUser"` for reads, `"WriteUser"` for writes |

Every request is user-scoped: the use case receives the current `User`, and persistence
filters on it. Never trust an id from the request to imply ownership.

## 2. The files

For `SearchStoreItems`, every file is named after the use case or the feature:

```
src/Holefeeder.Service/StoreItems/
    StoreItems.cs                                   AddStoreItemsServices, MapStoreItemsEndpoints
    Core/Code.cs                                    value object shared by the feature's use cases
    Core/SearchStoreItems/
        SearchStoreItemsUseCase.cs                  HandleAsync(request) -> Result<StoreItemsPage>
        SearchStoreItemsRequest.cs                  the use case's input: User + value objects
        SearchStoreItemsRequestFilterBy.cs          value object with its *Errors class
        SearchStoreItemsRequestSortBy.cs
        IStoreItems.cs                              the port
        StoreItem.cs, StoreItemsPage.cs             what the use case returns
    Persistence/
        StoreItemsInDatabase.cs                     implements IStoreItems; StoreItemsErrors
        StoreItemRow.cs                             DB row, maps to StoreItem
        SearchStoreItemsRequestWhereClause.cs       SQL built from the request
        SearchStoreItemsRequestOrderByClause.cs
    Api/SearchStoreItems/
        SearchStoreItemsEndpoint.cs                 MapSearchStoreItems + handler
        SearchStoreItemsRequestRaw.cs               query strings in, SearchStoreItemsRequest out
        StoreItemResponse.cs                        JSON out
```

Code used by a second feature moves to `Shared/<ring>` in the same change, never
before (`Shared/Core/Search`: `Offset`, `Limit`, `SearchQueryParser`, `PagedData`;
`Shared/Persistence/PagingClause`; `Shared/Api/ApiResult`). `Support/` holds the
current user (`User`, `UserContextAccessor`), which every feature may use.

## 3. Core

Core references no ring: no ASP.NET Core, no Npgsql, no Dapper, no DTOs.

**Value objects are always valid.** Private constructor; `Create` validates and returns
`Result<T>`; `Valid` skips validation and is only for test values and database reads.
Each one has an `*Errors` class next to it, with the code as a constant:

```csharp
internal sealed record Code
{
    private Code(string value) => Value = value;

    public string Value { get; }

    public static Result<Code> Create(string value) =>
        value.Length switch
        {
            0 or > 100 => CodeErrors.Invalid,
            _ => new Code(value)
        };

    public static Code Valid(string value) => new(value);
}

internal static class CodeErrors
{
    public const string CodeInvalid = "code-invalid";

    public static ResultError Invalid => new(CodeInvalid, "Code must be between 1 and 100 characters.");
}
```

Error codes are kebab-case and unique across the service; messages are sentences,
capitalized and ending with a period. `ErrorConventions` enforces both.

**The request** is a record of value objects plus the `User`, so the use case never sees
raw input:

```csharp
internal record SearchStoreItemsRequest(User User, Offset Offset, Limit Limit,
    SearchStoreItemsRequestFilterBy FilterBy, SearchStoreItemsRequestSortBy SortBy);
```

**The port** sits in the use case folder and returns `Result<T>`:

```csharp
internal interface IStoreItems
{
    Task<Result<StoreItemsPage>> SearchAsync(SearchStoreItemsRequest request, CancellationToken cancellationToken);
}
```

**The use case** is a class named `<UseCase>UseCase` with one `HandleAsync`. Business
rules go here; when there are none, it delegates to the port:

```csharp
internal sealed class SearchStoreItemsUseCase(IStoreItems storeItems)
{
    public Task<Result<StoreItemsPage>> HandleAsync(SearchStoreItemsRequest request, CancellationToken cancellationToken) =>
        storeItems.SearchAsync(request, cancellationToken);
}
```

## 4. Persistence

`<Feature>InDatabase` implements the port with Dapper, an injected `NpgsqlDataSource`
and hand-written SQL. Values always go through `DynamicParameters`; never interpolate
input into SQL. Only clause fragments built from known values (column names from a
fixed dictionary, operators from an enum) are interpolated.

```csharp
try
{
    await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);

    var total = await connection.ExecuteScalarAsync<int>(
        new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));
    var rows = await connection.QueryAsync<StoreItemRow>(
        new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

    return new StoreItemsPage(total, [.. rows.Select(row => row.ToStoreItem())]);
}
catch (NpgsqlException exception)
{
    LogReadFailed(logger, exception);
    return StoreItemsErrors.ReadFailed;
}
```

- Rows are records that map to Core types with `Valid` (`StoreItemRow.ToStoreItem`);
  Core entities are never persisted directly.
- Database exceptions are caught here, logged with a `[LoggerMessage]`, and returned
  as an `UnexpectedError` (`StoreItemsErrors.ReadFailed`). Nothing throws past the ring.
- Schema changes are DbUp scripts in `Holefeeder.Infrastructure/Scripts/` (see
  *Schema migrations* in `docs/plans/backend-service.md`).

## 5. Api

**The raw request** takes every input as a nullable string, so a malformed value
becomes a validation error rather than a binding failure. It validates every field,
collects all failures, and only then builds the Core request:

```csharp
internal sealed record SearchStoreItemsRequestRaw(
    [FromQuery(Name = "offset")] string? OffsetRaw,
    [FromQuery(Name = "limit")] string? LimitRaw,
    [FromQuery(Name = "sort")] string? SortRaw,
    [FromQuery(Name = "filter")] string? FilterRaw)
{
    public static Result<SearchStoreItemsRequest> ToSearchStoreItemsRequest(SearchStoreItemsRequestRaw raw, User user)
    {
        var offset = SearchQueryParser.ParseOrDefault(raw.OffsetRaw, Offset.Default, Offset.Create, OffsetErrors.Invalid);
        // limit, filter and sort the same way

        var validation = ResultAggregate.Create();
        if (offset.IsFailure)
        {
            validation.AddResult(offset.Error);
        }
        // one check per field

        return validation.OnSuccess<SearchStoreItemsRequest>(() =>
            new SearchStoreItemsRequest(user, offset.Value, limit.Value, filter.Value, sort.Value));
    }
}
```

**The endpoint** is a static class with a `Map<UseCase>` extension and a handler that
maps, calls the use case, and turns the result into HTTP with `ApiResult`:

```csharp
internal static class SearchStoreItemsEndpoint
{
    private const string ReadUserPolicy = "ReadUser";

    public static IEndpointRouteBuilder MapSearchStoreItems(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/v2/store-items", HandlerAsync)
            .Produces<IEnumerable<StoreItemResponse>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem()
            .WithTags("StoreItems")
            .WithName("SearchStoreItems")
            .RequireAuthorization(ReadUserPolicy);

        return app;
    }

    private static async Task<IResult> HandlerAsync(
        [AsParameters] SearchStoreItemsRequestRaw rawRequest,
        HttpContext context,
        UserContextAccessor userContextAccessor,
        SearchStoreItemsUseCase useCase,
        CancellationToken cancellationToken)
    {
        var requestResult = SearchStoreItemsRequestRaw.ToSearchStoreItemsRequest(rawRequest, userContextAccessor.Current);
        if (requestResult.IsFailure)
        {
            return ApiResult.ToProblem(requestResult.Error);
        }
        var request = requestResult.Value;

        var pagedResult = await useCase.HandleAsync(request, cancellationToken);
        if (pagedResult.IsFailure)
        {
            return ApiResult.ToProblem(pagedResult.Error);
        }
        var paged = pagedResult.Value;

        return ApiResult.ToPagedOk(paged.Total, paged.Items.Select(item => new StoreItemResponse(item.Id.Value, item.Code.Value, item.Data)), context);
    }
}
```

`ApiResult.ToProblem` decides the status, so no endpoint maps error codes itself:

| Error | Response |
|---|---|
| `ResultErrorAggregate` (from the raw request) | 400 validation problem, errors keyed by error code |
| `UnexpectedError` (database and other failures not caused by the caller) | 500, the error's description as detail |
| any other `ResultError` | 400, the error's description as detail |

A paged read answers with `ApiResult.ToPagedOk`, which writes `X-Total-Count`. The
response is a record of primitives (`StoreItemResponse`), never a Core type.

## 6. Wire the feature

`<Feature>.cs` at the feature root registers the use case and adapters, and maps the
endpoints; `Program.cs` calls both:

```csharp
internal static class StoreItems
{
    public static IServiceCollection AddStoreItemsServices(this IServiceCollection services) =>
        services
            .AddScoped<IStoreItems, StoreItemsInDatabase>()
            .AddScoped<SearchStoreItemsUseCase>();

    public static IEndpointRouteBuilder MapStoreItemsEndpoints(this IEndpointRouteBuilder app) =>
        app.MapSearchStoreItems();
}
```

## 7. Tests

All in `tests/Holefeeder.ServiceTests/<Feature>/`, mirroring the source folders. No
mocks: Core is tested against an in-memory port, Persistence against a real database.

| What | Where | How |
|---|---|---|
| Value objects | `Core/CodeTests.cs`, `Core/SearchStoreItems/*Tests.cs` | `Create` succeeds and fails with the right error |
| Use case | `Core/SearchStoreItems/SearchStoreItemsUseCaseTests.cs` | `StoreItemsInMemory` port, including `FailWith(error)` |
| Raw request | `Api/SearchStoreItems/SearchStoreItemsRequestRawTests.cs` | each malformed field gives its error code; several give them all |
| Persistence | `Persistence/StoreItemsInDatabaseTests.cs` | `[Collection(DatabaseTestMarker.Name)]`, `DatabaseForTest`, `BadDataSource` for the failure |
| Clause builders | `Persistence/SearchStoreItemsRequestOrderByClauseTests.cs` | plain unit tests |
| Endpoint | `Api/SearchStoreItems/SearchStoreItemsComponent.cs` | `[Collection(ComponentTestMarker.Name)]`, `ServerForTest`, FluentScenario, real HTTP |

Test data comes from the feature's `TestKit/`: `A<Thing>()` builders
(`AStoreItem`, `ACode`, `ASearchStoreItemsRequest`), `.SavedInDbAsync(...)` to persist,
and `<Feature>InMemory` for the port. The component test covers the happy path, each
validation failure, other users' data staying invisible, and an unregistered user.

`ArchitectureRules` (rings depend inward, features stay apart, no legacy namespaces)
and `ErrorConventions` run with the rest of the service tests.

## 8. Run everything

```bash
cd backend
dotnet test tests/Holefeeder.ServiceTests/Holefeeder.ServiceTests.csproj
dotnet test tests/Holefeeder.FunctionalTests/Holefeeder.FunctionalTests.csproj
dotnet test tests/Holefeeder.UnitTests/Holefeeder.UnitTests.csproj
dotnet format DrifterApps.Holefeeder.slnx --verify-no-changes --severity error
```

Database tests start Testcontainers. On rootless podman/macOS, set
`TESTCONTAINERS_RYUK_DISABLED=true` and point `DOCKER_HOST` at the podman machine
socket (see `backend/AGENTS.md`). One class:
`dotnet test <project> -- --filter-class "*SearchStoreItemsComponent"`.

## 9. Then the clients

An endpoint is not shipped until something calls it. Check whether the frontend
workspace (`frontend/`, see `frontend/AGENTS.md`) or the Angular SPA
(`backend/src/Holefeeder.Web/ClientApp`) needs the call, and whether
`docs/business-rules/` describes behavior the endpoint now implements.

## Never

- Add an endpoint to `Holefeeder.Application/Features/`.
- Reference a ring, ASP.NET Core or Npgsql from `Core`, or another feature's folders
  from anywhere.
- Throw for an expected failure; return a `ResultError`.
- Build a Core value without `Create`, except with `Valid` in tests and database reads.
- Interpolate request values into SQL.
- Query without filtering on the current user.
- Map error codes to status codes in an endpoint; `ApiResult` does it.
- Put code in `Shared` before a second feature needs it.
