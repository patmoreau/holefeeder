using System.Text.RegularExpressions;

using DrifterApps.Seeds.Testing.Scenarios;

using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;

namespace Holefeeder.FunctionalTests.Features;

/// <summary>
///     <P>Base class for scenario tests in this namespace</P>
///     <P>This class will clear the database using the Respawn package at beginning of every test.</P>
///     Implements Xunit.IAsyncLifetime to manage cleaning up after async tasks.
/// </summary>
public abstract partial class HolefeederScenario : Scenario
{
    private readonly ApiApplicationDriver _apiApplicationDriver;

    protected override async Task ResetStateAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _apiApplicationDriver.ResetStateAsync().ConfigureAwait(false);
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
        }
    }

    protected HolefeederScenario(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(apiApplicationDriver);

        _apiApplicationDriver = apiApplicationDriver;

        DatabaseDriver = _apiApplicationDriver.DatabaseDriver;

        Account = new AccountStepDefinition(HttpClientDriver, DatabaseDriver);
        Cashflow = new CashflowStepDefinition(HttpClientDriver, DatabaseDriver);
        Category = new CategoryStepDefinition(HttpClientDriver, DatabaseDriver);
        StoreItem = new StoreItemStepDefinition(HttpClientDriver);
        Transaction = new TransactionStepDefinition(HttpClientDriver);
        User = new UserStepDefinition(HttpClientDriver);
    }

    internal AccountStepDefinition Account { get; }

    internal CashflowStepDefinition Cashflow { get; }

    internal CategoryStepDefinition Category { get; }

    internal StoreItemStepDefinition StoreItem { get; }

    internal TransactionStepDefinition Transaction { get; }

    internal UserStepDefinition User { get; }


    protected BudgetingDatabaseDriver DatabaseDriver { get; }

#pragma warning disable S1135
    // TODO: remove
    protected void GivenUserIsUnauthorized() => HttpClientDriver.UnAuthenticate();

    // TODO: remove
    protected void GivenUserIsAuthorized() => HttpClientDriver.AuthenticateUser(TestUsers[AuthorizedUser].IdentityObjectId);
#pragma warning restore S1135

    protected static Guid ResourceIdFromLocation(Uri location)
    {
        ArgumentNullException.ThrowIfNull(location);

        var match = ResourceIdRegex().Match(location.ToString());

        return match.Success ? Guid.Parse(match.Value) : Guid.Empty;
    }

    [GeneratedRegex("[{(]?[0-9A-Fa-f]{8}[-]?([0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}[)}]?")]
    private static partial Regex ResourceIdRegex();
}
