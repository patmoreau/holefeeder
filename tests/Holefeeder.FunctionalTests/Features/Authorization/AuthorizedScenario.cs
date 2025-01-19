using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;

namespace Holefeeder.FunctionalTests.Features.Authorization;

public class AuthorizedScenario(ApiApplicationSecurityDriver appDriver, ITestOutputHelper testOutputHelper) :
    SecurityScenario(testOutputHelper)
{
    private readonly AuthenticatedUserSteps _user = new(appDriver);

    [Fact]
    public Task AttemptToCloseAccount() => ScenarioForAuthorizedUser("close an account", _user.ClosingAnAccount);

    [Fact]
    public Task AttemptToSetFavoriteAccount() =>
        ScenarioForAuthorizedUser("set a favorite account", _user.SettingAFavoriteAccount);

    [Fact]
    public Task AttemptToModifyAnAccount() => ScenarioForAuthorizedUser("modify an account", _user.ModifyingAnAccount);

    [Fact]
    public Task AttemptToOpenAnAccount() => ScenarioForAuthorizedUser("open an account", _user.OpeningAnAccount);

    [Fact]
    public Task AttemptToGetAccount() => ScenarioForAuthorizedUser("get an account", _user.GettingAnAccount);

    [Fact]
    public Task AttemptToGetAccounts() => ScenarioForAuthorizedUser("get a list of accounts", _user.GettingAccounts);

    [Fact]
    public Task AttemptToGetCategories() => ScenarioForAuthorizedUser("get a list of categories", _user.GettingCategories);

    [Fact]
    public Task AttemptToImportData() => ScenarioForAuthorizedUser("import data", _user.ImportingData);

    [Fact]
    public Task AttemptToExportData() => ScenarioForAuthorizedUser("export data", _user.ExportingData);

    [Fact]
    public Task AttemptToGetImportDataStatus() =>
        ScenarioForAuthorizedUser("get import data status", _user.GettingImportStatus);

    [Fact]
    public Task AttemptToGetStatisticsForAllCategories() =>
        ScenarioForAuthorizedUser("get statistics for all categories", _user.GettingStatisticsForAllCategories);

    [Fact]
    public Task AttemptToGetStatisticsSummary() =>
        ScenarioForAuthorizedUser("get statistics summary", _user.GettingStatisticsSummary);

    [Fact]
    public Task AttemptToGetStoreItems() =>
        ScenarioForAuthorizedUser("get store items", _user.GettingItemsFromTheStore);

    [Fact]
    public Task AttemptToGetStoreItem() =>
        ScenarioForAuthorizedUser("get store item", _user.GettingAnItemFromTheStore);

    [Fact]
    public Task AttemptToCreateStoreItem() =>
        ScenarioForAuthorizedUser("create a store item", _user.CreatingAnItemInTheStore);

    [Fact]
    public Task AttemptToModifyStoreItem() =>
        ScenarioForAuthorizedUser("modify a store item", _user.ModifyingAnItemInTheStore);

    [Fact]
    public Task AttemptToGetTags() => ScenarioForAuthorizedUser("get tags", _user.GettingTags);

    [Fact]
    public Task AttemptToGetCashflows() => ScenarioForAuthorizedUser("get cashflows", _user.GettingCashflows);

    [Fact]
    public Task AttemptToGetCashflow() => ScenarioForAuthorizedUser("get cashflow", _user.GettingACashflow);

    [Fact]
    public Task AttemptToModifyCashflow() => ScenarioForAuthorizedUser("modify cashflow", _user.ModifyingACashflow);

    [Fact]
    public Task AttemptToCancelCashflow() => ScenarioForAuthorizedUser("cancel cashflow", _user.CancellingACashflow);

    [Fact]
    public Task AttemptToGetUpcoming() => ScenarioForAuthorizedUser("get upcoming", _user.GettingUpcomingCashflows);

    [Fact]
    public Task AttemptToMakePurchase() => ScenarioForAuthorizedUser("make purchase", _user.MakingAPurchase);

    [Fact]
    public Task AttemptToPayCashflow() => ScenarioForAuthorizedUser("pay cashflow", _user.PayingACashflow);

    [Fact]
    public Task AttemptToTransfer() => ScenarioForAuthorizedUser("transfer", _user.TransferringMoneyBetweenTwoAccounts);

    [Fact]
    public Task AttemptToModifyTransaction() => ScenarioForAuthorizedUser("modify transaction", _user.ModifyingATransaction);

    [Fact]
    public Task AttemptToDeleteTransaction() => ScenarioForAuthorizedUser("delete transaction", _user.DeletingATransaction);

    [Fact]
    public Task AttemptToGetTransaction() => ScenarioForAuthorizedUser("get transaction", _user.GettingATransaction);

    [Fact]
    public Task AttemptToGetTransactions() => ScenarioForAuthorizedUser("get transactions", _user.GettingTransactions);
}
