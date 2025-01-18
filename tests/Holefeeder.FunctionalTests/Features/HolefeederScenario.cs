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
public abstract class HolefeederScenario : BaseScenario, IAsyncLifetime
{
    private readonly ApiApplicationDriver _apiApplicationDriver;

    protected HolefeederScenario(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(apiApplicationDriver);

        _apiApplicationDriver = apiApplicationDriver;

        DatabaseDriver = _apiApplicationDriver.DatabaseDriver;

        TheUser = new UserSteps(_apiApplicationDriver);
        TheData = new DataSteps(DatabaseDriver);
        Account = new AccountSteps(DatabaseDriver);
        Cashflow = new CashflowSteps(DatabaseDriver);
        Category = new CategorySteps(DatabaseDriver);
        StoreItem = new StoreItemSteps(DatabaseDriver);
        Transaction = new TransactionSteps(DatabaseDriver);
    }

    internal UserSteps TheUser { get; }

    internal DataSteps TheData { get; }

    internal AccountSteps Account { get; }

    internal CashflowSteps Cashflow { get; }

    internal CategorySteps Category { get; }

    internal StoreItemSteps StoreItem { get; }

    internal TransactionSteps Transaction { get; }

    protected BudgetingDatabaseDriver DatabaseDriver { get; }

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

    [AssertionMethod]
    protected static void ShouldNotBeFound(IStepRunner runner) =>
        runner.Execute<IApiResponse>(response =>
            response.Should().BeValid()
                .And.Subject.Value.Should().HaveStatusCode(HttpStatusCode.NotFound));

    public Task InitializeAsync() => _apiApplicationDriver.ResetStateAsync();

    public Task DisposeAsync() => Task.CompletedTask;
}
