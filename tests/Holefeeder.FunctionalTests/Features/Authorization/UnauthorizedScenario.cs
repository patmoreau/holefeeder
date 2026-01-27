using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;

namespace Holefeeder.FunctionalTests.Features.Authorization;

public class UnauthorizedScenario(ApiApplicationSecurityDriver appDriver, ITestOutputHelper testOutputHelper) :
    SecurityScenario(testOutputHelper)
{
    private readonly UnauthenticatedUserSteps _user = new(appDriver);

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
    public Task AttemptToGetAccount() => ScenarioForUnauthenticatedUser("get an account", _user.GettingAnAccount);

    [Fact]
    public Task AttemptToGetAccounts() => ScenarioForUnauthenticatedUser("get a list of accounts", _user.GettingAccounts);

    [Fact]
    public Task AttemptToGetCategories() =>
        ScenarioForUnauthenticatedUser("get a list of categories", _user.GettingCategories);

    [Fact]
    public Task AttemptToGetAccountTypes() =>
        ScenarioForAuthorizedUser("get the list of account types", _user.GettingAccountTypes);

    [Fact]
    public Task AttemptToGetCategoryTypes() =>
        ScenarioForAuthorizedUser("get the list of category types", _user.GettingCategoryTypes);

    [Fact]
    public Task AttemptToGetDateIntervalTypes() =>
        ScenarioForAuthorizedUser("get the list of date interval types", _user.GettingDateIntervalTypes);

    [Fact]
    public Task AttemptToImportData() => ScenarioForUnauthenticatedUser("import data", _user.ImportingData);

    [Fact]
    public Task AttemptToExportData() => ScenarioForUnauthenticatedUser("export data", _user.ExportingData);

    [Fact]
    public Task AttemptToGetImportDataStatus() =>
        ScenarioForUnauthenticatedUser("get import data status", _user.GettingImportStatus);

    [Fact]
    public Task AttemptToComputePeriod() => ScenarioForUnauthenticatedUser("compute period", _user.ComputePeriod);

    [Fact]
    public Task AttemptToGetDashboardSummary() =>
        ScenarioForUnauthenticatedUser("get dashboard summary", _user.GettingDashboardSummary);

    [Fact]
    public Task AttemptToGetStatisticsForAllCategories() =>
        ScenarioForUnauthenticatedUser("get statistics for all categories", _user.GettingStatisticsForAllCategories);

    [Fact]
    public Task AttemptToGetStatisticsSummary() =>
        ScenarioForUnauthenticatedUser("get statistics summary", _user.GettingStatisticsSummary);

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
    public Task AttemptToGetTags() => ScenarioForUnauthenticatedUser("get tags", _user.GettingTags);

    [Fact]
    public Task AttemptToGetCashflows() => ScenarioForUnauthenticatedUser("get cashflows", _user.GettingCashflows);

    [Fact]
    public Task AttemptToGetCashflow() => ScenarioForUnauthenticatedUser("get cashflow", _user.GettingACashflow);

    [Fact]
    public Task AttemptToModifyCashflow() => ScenarioForUnauthenticatedUser("modify cashflow", _user.ModifyingACashflow);

    [Fact]
    public Task AttemptToCancelCashflow() => ScenarioForUnauthenticatedUser("cancel cashflow", _user.CancellingACashflow);

    [Fact]
    public Task AttemptToGetUpcoming() => ScenarioForUnauthenticatedUser("get upcoming", _user.GettingUpcomingCashflows);

    [Fact]
    public Task AttemptToMakePurchase() => ScenarioForUnauthenticatedUser("make purchase", _user.MakingAPurchase);

    [Fact]
    public Task AttemptToPayCashflow() => ScenarioForUnauthenticatedUser("pay cashflow", _user.PayingACashflow);

    [Fact]
    public Task AttemptToTransfer() => ScenarioForUnauthenticatedUser("transfer", _user.TransferringMoneyBetweenTwoAccounts);

    [Fact]
    public Task AttemptToModifyTransaction() =>
        ScenarioForUnauthenticatedUser("modify transaction", _user.ModifyingATransaction);

    [Fact]
    public Task AttemptToDeleteTransaction() =>
        ScenarioForUnauthenticatedUser("delete transaction", _user.DeletingATransaction);

    [Fact]
    public Task AttemptToGetTransaction() => ScenarioForUnauthenticatedUser("get transaction", _user.GettingATransaction);

    [Fact]
    public Task AttemptToGetTransactions() => ScenarioForUnauthenticatedUser("get transactions", _user.GettingTransactions);

    [Fact]
    public Task AttemptToSync() => ScenarioForUnauthenticatedUser("syncs", _user.Sync);
}
