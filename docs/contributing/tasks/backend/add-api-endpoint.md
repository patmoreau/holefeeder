---
name: add-api-endpoint
description: Add a REST endpoint to the Holefeeder API as a Carter vertical slice, with validator, error mapping and tests
---

# Add a REST endpoint to the Holefeeder backend

> **Audience:** contributors *working on* this repository. Not a published document — see `AGENTS.md` at the repository root.

Endpoints are vertical slices: **one file holds the route, the request, the validator and
the handler**, and it lives in `backend/src/Holefeeder.Application/Features/<Feature>/`
under `Commands/` or `Queries/` — not in `Holefeeder.Api`. Carter discovers every
`ICarterModule` in the Application assembly (`AddCarter` / `MapCarter` in
`src/Holefeeder.Api/Program.cs`), so a new slice needs no registration.

Read `backend/CLAUDE.md` first: TDD is required here, which means stating the test plan
before writing implementation code.

## 1. Decide the shape

| Question | Convention |
|---|---|
| Feature | An existing folder under `Features/` (`Accounts`, `Transactions`, `StoreItems`, `Tags`, …), or a new one when the concept is new |
| Command or query | Mutations go in `Commands/`, reads in `Queries/` |
| File and class name | The use case, in PascalCase: `OpenAccount`, `GetStoreItems`, `PayCashflow` |
| Route | Collection read `api/v2/<feature-kebab>`; single read `api/v2/<feature-kebab>/{id:guid}`; command `api/v2/<feature-kebab>/<use-case-kebab>` (`api/v2/accounts/open-account`), `MapDelete` for deletes |
| Policy | `Policies.ReadUser` for queries, `Policies.WriteUser` for commands |

Every route is user-scoped: filter by `userContext.Id` in the handler. Never trust an id
from the request to imply ownership.

## 2. Write the slice

A command — note the two endpoint filters, the `Result<T>` return and the nested types:

```csharp
public class OpenAccount : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapPost("api/v2/accounts/open-account",
                async (Request request, IUserContext userContext, BudgetingContext context,
                    CancellationToken cancellationToken) =>
                {
                    var result = await Handle(request, userContext, context, cancellationToken);
                    return result switch
                    {
                        { IsFailure: true } => result.Error.ToProblem(),
                        _ => Results.CreatedAtRoute(nameof(GetAccount), new { Id = (Guid)result.Value },
                            new { Id = (Guid)result.Value })
                    };
                })
            .AddEndpointFilter<ValidationFilter<Request>>()   // 422 before the handler
            .AddEndpointFilter<UnitOfWorkFilter>()            // transaction around it
            .Produces<Guid>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithTags(nameof(Accounts))
            .WithName(nameof(OpenAccount))
            .RequireAuthorization(Policies.WriteUser);

    private static async Task<Result<AccountId>> Handle(Request request, IUserContext userContext,
        BudgetingContext context, CancellationToken cancellationToken)
    {
        // Guard clauses return a domain error; the happy path returns the value and the
        // implicit conversion to Result<T> does the rest.
        if (await context.Accounts.AnyAsync(x => x.Name == request.Name && x.UserId == userContext.Id,
                cancellationToken))
        {
            return AccountErrors.NameAlreadyExists(request.Name);
        }

        var result = Account.Create(request.Type, request.Name, request.OpenBalance, request.OpenDate,
            request.Description, userContext.Id);   // signature per the domain factory
        if (result.IsFailure)
        {
            return result.Error;
        }

        await context.Accounts.AddAsync(result.Value, cancellationToken);

        return result.Value.Id;
    }

    public record Request(AccountType Type, string Name, DateOnly OpenDate, Money OpenBalance,
        string Description);

    internal class Validator : AbstractValidator<Request>
    {
        public Validator() => RuleFor(x => x.Name).NotEmpty();
    }
}
```

The slice carries its own `using` block — `DrifterApps.Seeds.Application.EndpointFilters`,
`DrifterApps.Seeds.FluentResult`, the `Holefeeder.Application.*` and
`Holefeeder.Domain.Features.*` namespaces it touches, plus the ASP.NET Core ones. There is
no `using Carter`, `using FluentValidation` or import for `Policies`: the Application
project's `GlobalUsings.cs` already covers those (`Carter`, `FluentValidation`, and
`static Holefeeder.Application.Authorization.Configuration`).

`Request` is `public` when a test builder or the step definitions reference it
(`CreateStoreItem.Request`), `internal` otherwise. `Validator` is always `internal`, and
Carter's `WithEmptyValidators()` configuration means a slice with no validator is fine —
but a command that takes input should have one.

A paged query differs in four places: `QueryParams.Create`, the `BindAsync` binder, the
`X-Total-Count` header and `QueryValidatorRoot`:

```csharp
public class GetAccounts : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app) =>
        app.MapGet("api/v2/accounts",
                async (Request request, IUserContext userContext, BudgetingContext context,
                    HttpContext ctx, CancellationToken cancellationToken) =>
                {
                    var result = await Handle(request, userContext, context, cancellationToken);
                    switch (result)
                    {
                        case { IsFailure: true }:
                            return result.Error.ToProblem();
                        default:
                            ctx.Response.Headers.Append("X-Total-Count", $"{result.Value.Total}");
                            return Results.Ok(result.Value.Items);
                    }
                })
            .AddEndpointFilter<ValidationFilter<Request>>()
            .Produces<QueryResult<Response>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .WithMetadata(nameof(IRequestQuery))
            .WithTags(nameof(Accounts))
            .WithName(nameof(GetAccounts))
            .RequireAuthorization(Policies.ReadUser);

    private static async Task<Result<QueryResult<Response>>> Handle(Request request,
        IUserContext userContext, BudgetingContext context, CancellationToken cancellationToken)
    {
        var queryParams = QueryParams.Create(request);
        if (queryParams.IsFailure)
        {
            return queryParams.Error;
        }

        var total = await context.Accounts.Where(e => e.UserId == userContext.Id)
            .CountAsync(cancellationToken);
        var items = await context.Accounts
            .Where(e => e.UserId == userContext.Id)
            .Query(queryParams.Value)
            .Select(e => new Response(e.Id, e.Name))
            .ToListAsync(cancellationToken);

        return new QueryResult<Response>(total, items);
    }

    internal record Request(int Offset, int Limit, string[] Sort, string[] Filter) : IRequestQuery
    {
        public static ValueTask<Request?> BindAsync(HttpContext context, ParameterInfo parameter) =>
            context.ToQueryRequest((offset, limit, sort, filter) => new Request(offset, limit, sort, filter));
    }

    internal record Response(Guid Id, string Name);

    internal class Validator : QueryValidatorRoot<Request>;
}
```

`QueryValidatorRoot<Request>` already covers `Offset`/`Limit`/`Sort`/`Filter` — only add
rules for the columns this endpoint accepts.

## 3. Wire the failure codes

Handlers return `ResultError`, never throw, and never build an `IResult` themselves.
Domain errors are static members on the feature's `*Errors` class in
`Holefeeder.Domain/Features/<Feature>/`, with a `Code` constant so the mapper can match:

```csharp
public const string CodeNotFound = $"{nameof(Account)}.{nameof(NotFound)}";

public static ResultError NotFound(AccountId id) => new(CodeNotFound, $"Account '{id}' not found");
```

Then add the code to the switch in
`Holefeeder.Application/Extensions/ResultErrorExtensions.cs` so `ToProblem()` maps it to
the right status. **An unmapped code falls through to 500**, which is the usual cause of
"my 404 returns a 500".

## 4. Expose it to the functional tests

`tests/Holefeeder.FunctionalTests/Infrastructure/IUser.cs` is the Refit contract the
scenarios call. Add the method to the matching interface:

```csharp
[Post("/api/v2/accounts/open-account")]
Task<IApiResponse> OpenAccountAsync([Body] object request);
```

Then add a step to the relevant `StepDefinitions/*Steps.cs` (usually `UserSteps.cs`):

```csharp
internal void OpensAnAccount(IStepRunner runner) =>
    runner.Execute<OpenAccount.Request, IApiResponse>(request =>
    {
        request.Should().BeValid();
        return Api.OpenAccountAsync(request.Value);
    });
```

## 5. Build the request builder

Request fakes live in `tests/Holefeeder.Tests.Common/Builders/<Feature>/`, one
`FakerBuilder<Request>` per request, with `With…` / `WithNo…` mutators returning `this`:

```csharp
internal class OpenAccountRequestBuilder : FakerBuilder<Request>
{
    protected override Faker<Request> Faker { get; } = CreateUninitializedFaker<Request>()
        .RuleFor(x => x.Name, faker => faker.Random.Hash());

    public static OpenAccountRequestBuilder GivenAnOpenAccountRequest() => new();

    public OpenAccountRequestBuilder WithNoName()
    {
        Faker.RuleFor(x => x.Name, string.Empty);
        return this;
    }
}
```

## 6. Write the tests first

Per `backend/CLAUDE.md`: no mocks, real test database, test plan stated before
implementation. Two suites, both required for a new endpoint:

**Validator unit tests** —
`tests/Holefeeder.UnitTests/Application/Features/<Feature>/<Commands|Queries>/<UseCase>Tests.cs`:

```csharp
[UnitTest]
public class OpenAccountTests
{
    [Fact]
    public async Task GivenValidator_WhenNameIsEmpty_ThenError()
    {
        // arrange
        var request = GivenAnOpenAccountRequest().WithNoName().Build();
        var validator = new Validator();

        // act
        var result = await validator.TestValidateAsync(request,
            cancellationToken: TestContext.Current.CancellationToken);

        // assert
        result.ShouldHaveValidationErrorFor(r => r.Name);
    }
}
```

**Functional scenario** —
`tests/Holefeeder.FunctionalTests/Features/<Feature>/Scenario<UseCase>.cs`, one `[Fact]`
per outcome, built from `ScenarioRunner`:

```csharp
public class ScenarioOpenAccount(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.OpensAnAccount)
            .Then(ShouldExpectBadRequest)
            .PlayAsync();

    [Fact]
    public Task WhenAccountOpened() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AValidRequest)
            .When(TheUser.OpensAnAccount)
            .Then(ShouldExpectStatusCodeCreated)
            .PlayAsync();
}
```

Cover, at minimum: invalid request (422/400), the happy path, each domain failure the
handler can return, and unauthorized access for the policy the route requires.

## 7. Run everything

```bash
cd backend
dotnet build DrifterApps.Holefeeder.slnx
dotnet test tests/Holefeeder.UnitTests/Holefeeder.UnitTests.csproj
dotnet test tests/Holefeeder.FunctionalTests/Holefeeder.FunctionalTests.csproj
dotnet format DrifterApps.Holefeeder.slnx --severity error
```

Functional tests start Testcontainers. On rootless podman/macOS, Ryuk fails with
`making volume mountpoint … operation not supported` — run them with
`TESTCONTAINERS_RYUK_DISABLED=true` and a `DOCKER_HOST` pointing at the podman machine
socket (see `backend/CLAUDE.md`). `--settings .runsettings` reports "Zero tests ran";
pass the project path instead.

## 8. Then the clients

A new endpoint is not shipped until something calls it. Check whether the Angular SPA
(`backend/src/Holefeeder.Web/ClientApp`) or the frontend workspace (`frontend/`, see
`frontend/CLAUDE.md`) needs the corresponding client call, and whether
`docs/business-rules/` describes behavior this endpoint now implements.

## Never

- Put the route in `Holefeeder.Api` — slices live in `Holefeeder.Application/Features/`.
- Throw for an expected failure, or return `IResult` from `Handle`.
- Skip `UnitOfWorkFilter` on a mutating endpoint, or place it before `ValidationFilter<Request>`.
- Query without filtering on `userContext.Id`.
- Add a domain error code without mapping it in `ResultErrorExtensions.ToProblem`.
- Ship an endpoint with no functional scenario.
