using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;

namespace Holefeeder.FunctionalTests.Features.Authorization;

public class UnauthorizedScenario(ApiApplicationSecurityDriver appDriver, ITestOutputHelper testOutputHelper) :
    SecurityScenario(testOutputHelper)
{
    private readonly UnauthenticatedUserSteps _user = new(appDriver);

    [Fact]
    public Task AttemptToGetTheCurrentUser() =>
        ScenarioForUnauthenticatedUser("get the current user", _user.GettingTheCurrentUser);

    [Fact]
    public Task AttemptToRegisterTheUser() =>
        ScenarioForUnauthenticatedUser("register the user", _user.RegisteringTheUser);

    [Fact]
    public Task AttemptToCloseAccount() => ScenarioForUnauthenticatedUser("close an account", _user.ClosingAnAccount);

    [Fact]
    public Task AttemptToSetFavoriteAccount() =>
        ScenarioForUnauthenticatedUser("set a favorite account", _user.SettingAFavoriteAccount);

    [Fact]
    public Task AttemptToModifyAnAccount() => ScenarioForUnauthenticatedUser("modify an account", _user.ModifyingAnAccount);

    [Fact]
    public Task AttemptToOpenAnAccount() => ScenarioForUnauthenticatedUser("open an account", _user.OpeningAnAccount);

    [Fact]
    public Task AttemptToImportData() => ScenarioForUnauthenticatedUser("import data", _user.ImportingData);

    [Fact]
    public Task AttemptToExportData() => ScenarioForUnauthenticatedUser("export data", _user.ExportingData);

    [Fact]
    public Task AttemptToGetImportDataStatus() =>
        ScenarioForUnauthenticatedUser("get import data status", _user.GettingImportStatus);

    [Fact]
    public Task AttemptToGetDashboardSummary() =>
        ScenarioForUnauthenticatedUser("get dashboard summary", _user.GettingDashboardSummary);

    [Fact]
    public Task AttemptToGetStoreItems() =>
        ScenarioForUnauthenticatedUser("get store items", _user.GettingItemsFromTheStore);

    [Fact]
    public Task AttemptToGetStoreItem() =>
        ScenarioForUnauthenticatedUser("get store item", _user.GettingAnItemFromTheStore);

    [Fact]
    public Task AttemptToCreateStoreItem() =>
        ScenarioForUnauthenticatedUser("create a store item", _user.CreatingAnItemInTheStore);

    [Fact]
    public Task AttemptToModifyStoreItem() =>
        ScenarioForUnauthenticatedUser("modify a store item", _user.ModifyingAnItemInTheStore);

    [Fact]
    public Task AttemptToModifyCashflow() => ScenarioForUnauthenticatedUser("modify cashflow", _user.ModifyingACashflow);

    [Fact]
    public Task AttemptToModifyTransaction() =>
        ScenarioForUnauthenticatedUser("modify transaction", _user.ModifyingATransaction);

    [Fact]
    public Task AttemptToDeleteTransaction() =>
        ScenarioForUnauthenticatedUser("delete transaction", _user.DeletingATransaction);

    [Fact]
    public Task AttemptToGetTransaction() => ScenarioForUnauthenticatedUser("get transaction", _user.GettingATransaction);

    [Fact]
    public Task AttemptToSync() => ScenarioForUnauthenticatedUser("syncs", _user.Sync);
}
