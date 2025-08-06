using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;

namespace Holefeeder.FunctionalTests.Features.Authorization;

public class ForbiddenScenario(ApiApplicationSecurityDriver appDriver, ITestOutputHelper testOutputHelper) :
    SecurityScenario(testOutputHelper)
{
    private readonly ForbiddenUserSteps _user = new(appDriver);

    [Fact]
    public Task AttemptToCloseAccount() => ScenarioForForbiddenUser("close an account", _user.ClosingAnAccount);

    [Fact]
    public Task AttemptToSetFavoriteAccount() =>
        ScenarioForForbiddenUser("set a favorite account", _user.SettingAFavoriteAccount);

    [Fact]
    public Task AttemptToModifyAnAccount() => ScenarioForForbiddenUser("modify an account", _user.ModifyingAnAccount);

    [Fact]
    public Task AttemptToOpenAnAccount() => ScenarioForForbiddenUser("open an account", _user.OpeningAnAccount);

    [Fact]
    public Task AttemptToGetAccount() => ScenarioForForbiddenUser("get an account", _user.GettingAnAccount);

    [Fact]
    public Task AttemptToGetAccounts() => ScenarioForForbiddenUser("get a list of accounts", _user.GettingAccounts);

    [Fact]
    public Task AttemptToGetCategories() => ScenarioForForbiddenUser("get a list of categories", _user.GettingCategories);

    [Fact]
    public Task AttemptToImportData() => ScenarioForForbiddenUser("import data", _user.ImportingData);

    [Fact]
    public Task AttemptToExportData() => ScenarioForForbiddenUser("export data", _user.ExportingData);

    [Fact]
    public Task AttemptToGetImportDataStatus() =>
        ScenarioForForbiddenUser("get import data status", _user.GettingImportStatus);

    [Fact]
    public Task AttemptToComputePeriod() =>
        ScenarioForForbiddenUser("compute period", _user.ComputePeriod);

    [Fact]
    public Task AttemptToGetStatisticsForAllCategories() =>
        ScenarioForForbiddenUser("get statistics for all categories", _user.GettingStatisticsForAllCategories);

    [Fact]
    public Task AttemptToGetStatisticsSummary() =>
        ScenarioForForbiddenUser("get statistics summary", _user.GettingStatisticsSummary);

    [Fact]
    public Task AttemptToGetStoreItems() =>
        ScenarioForForbiddenUser("get store items", _user.GettingItemsFromTheStore);

    [Fact]
    public Task AttemptToGetStoreItem() =>
        ScenarioForForbiddenUser("get store item", _user.GettingAnItemFromTheStore);

    [Fact]
    public Task AttemptToCreateStoreItem() =>
        ScenarioForForbiddenUser("create a store item", _user.CreatingAnItemInTheStore);

    [Fact]
    public Task AttemptToModifyStoreItem() =>
        ScenarioForForbiddenUser("modify a store item", _user.ModifyingAnItemInTheStore);

    [Fact]
    public Task AttemptToGetTags() => ScenarioForForbiddenUser("get tags", _user.GettingTags);

    [Fact]
    public Task AttemptToGetCashflows() => ScenarioForForbiddenUser("get cashflows", _user.GettingCashflows);

    [Fact]
    public Task AttemptToGetCashflow() => ScenarioForForbiddenUser("get cashflow", _user.GettingACashflow);

    [Fact]
    public Task AttemptToModifyCashflow() => ScenarioForForbiddenUser("modify cashflow", _user.ModifyingACashflow);

    [Fact]
    public Task AttemptToCancelCashflow() => ScenarioForForbiddenUser("cancel cashflow", _user.CancellingACashflow);

    [Fact]
    public Task AttemptToGetUpcoming() => ScenarioForForbiddenUser("get upcoming", _user.GettingUpcomingCashflows);

    [Fact]
    public Task AttemptToMakePurchase() => ScenarioForForbiddenUser("make purchase", _user.MakingAPurchase);

    [Fact]
    public Task AttemptToPayCashflow() => ScenarioForForbiddenUser("pay cashflow", _user.PayingACashflow);

    [Fact]
    public Task AttemptToTransfer() => ScenarioForForbiddenUser("transfer", _user.TransferringMoneyBetweenTwoAccounts);

    [Fact]
    public Task AttemptToModifyTransaction() => ScenarioForForbiddenUser("modify transaction", _user.ModifyingATransaction);

    [Fact]
    public Task AttemptToDeleteTransaction() => ScenarioForForbiddenUser("delete transaction", _user.DeletingATransaction);

    [Fact]
    public Task AttemptToGetTransaction() => ScenarioForForbiddenUser("get transaction", _user.GettingATransaction);

    [Fact]
    public Task AttemptToGetTransactions() => ScenarioForForbiddenUser("get transactions", _user.GettingTransactions);
}
