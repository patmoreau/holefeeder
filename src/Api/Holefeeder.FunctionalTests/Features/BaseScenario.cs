using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;
using Holefeeder.Tests.Common.SeedWork.Scenarios;
using Microsoft.Extensions.DependencyInjection;

namespace Holefeeder.FunctionalTests.Features;

/// <summary>
///     <P>Base class for scenario tests in this namespace</P>
///     <P>This class will clear the database using the Respawn package at beginning of every test.</P>
///     Implements Xunit.IAsyncLifetime to manage cleaning up after async tasks.
/// </summary>
[Collection("Api collection")]
public abstract class BaseScenario : RootScenario
{
    private BudgetingDatabaseDriver? _databaseDriver;

    protected BaseScenario(ApiApplicationDriver apiApplicationDriver,
        BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper) : base(
        apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
        ArgumentNullException.ThrowIfNull(apiApplicationDriver);

        Scope = apiApplicationDriver.Services.CreateScope();

        StoreItem = new StoreItemStepDefinition(HttpClientDriver);
        Transaction = new TransactionStepDefinition(HttpClientDriver);
    }

    private IServiceScope Scope { get; }

    protected BudgetingDatabaseDriver DatabaseDriver =>
        _databaseDriver ??= Scope.ServiceProvider.GetRequiredService<BudgetingDatabaseDriver>();

    protected StoreItemStepDefinition StoreItem { get; }
    protected TransactionStepDefinition Transaction { get; }

    protected void GivenUserIsUnauthorized() => HttpClientDriver.UnAuthenticate();

    protected void GivenUserIsAuthorized() => HttpClientDriver.Authenticate();
}
