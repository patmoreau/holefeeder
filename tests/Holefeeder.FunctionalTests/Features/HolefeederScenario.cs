using System.Net;
using System.Text.RegularExpressions;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.Testing.Attributes;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;

using Refit;

namespace Holefeeder.FunctionalTests.Features;

/// <summary>
///     <P>Base class for scenario tests in this namespace</P>
///     <P>This class will clear the database using the Respawn package at beginning of every test.</P>
///     Implements Xunit.IAsyncLifetime to manage cleaning up after async tasks.
/// </summary>
[ComponentTest]
[Collection(FunctionalTestMarker.Name)]
public abstract partial class HolefeederScenario : BaseScenario, IAsyncLifetime
{
    private readonly ApiApplicationDriver _apiApplicationDriver;

    protected HolefeederScenario(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(apiApplicationDriver);

        _apiApplicationDriver = apiApplicationDriver;

        DatabaseDriver = _apiApplicationDriver.DatabaseDriver;

        Account = new AccountSteps(DatabaseDriver);
        // Cashflow = new CashflowStepDefinition(HttpClientDriver, DatabaseDriver);
        Category = new CategoryStepDefinition(DatabaseDriver);
        // StoreItem = new StoreItemStepDefinition(HttpClientDriver);
        // Transaction = new TransactionStepDefinition(HttpClientDriver);
        // User = new UserStepDefinition(HttpClientDriver);
    }

    protected UserSteps TheUser => new(_apiApplicationDriver);

    internal DataSteps TheData => new(DatabaseDriver);
    internal AccountSteps Account { get; }

    // internal CashflowStepDefinition Cashflow { get; }

    internal CategoryStepDefinition Category { get; }

    // internal StoreItemStepDefinition StoreItem { get; }

    // internal TransactionStepDefinition Transaction { get; }

    // internal UserStepDefinition User { get; }


    protected BudgetingDatabaseDriver DatabaseDriver { get; }

#pragma warning disable S1135
    // TODO: remove
    // protected void GivenUserIsUnauthorized() => HttpClientDriver.UnAuthenticate();

    // TODO: remove
    // protected void GivenUserIsAuthorized() => HttpClientDriver.AuthenticateUser(TestUsers[AuthorizedUser].IdentityObjectId);
#pragma warning restore S1135

    protected static Guid ResourceIdFromLocation(Uri location)
    {
        ArgumentNullException.ThrowIfNull(location);

        var match = ResourceIdRegex().Match(location.ToString());

        return match.Success ? Guid.Parse(match.Value) : Guid.Empty;
    }

    [AssertionMethod]
    protected static void ShouldReceiveAValidationError(IStepRunner runner) =>
        runner.Execute<IApiResponse>(response =>
        {
            response.Should().BeValid()
                .And.Subject
                .Value.Should().BeFailure()
                .And.HaveError("One or more validation errors occurred.");
        });

    [AssertionMethod]
    protected static void ShouldExpectBadRequest(IStepRunner runner) =>
        runner.Execute<IApiResponse>(response =>
        {
            response.Should().BeValid()
                .And.Subject
                .Value.Should().BeFailure().And.HaveStatusCode(HttpStatusCode.BadRequest);
        });

    [GeneratedRegex("[{(]?[0-9A-Fa-f]{8}[-]?([0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}[)}]?")]
    private static partial Regex ResourceIdRegex();

    public Task InitializeAsync() => _apiApplicationDriver.ResetStateAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [AssertionMethod]
    protected static void ShouldNotBeFound(IStepRunner runner) =>
        runner.Execute<IApiResponse>(response =>
            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.NotFound));
}
