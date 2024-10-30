using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;

namespace Holefeeder.FunctionalTests.Features.Authorization;

public class AuthorizedScenario(ApiApplicationSecurityDriver appDriver, ITestOutputHelper testOutputHelper) :
    SecurityScenario(testOutputHelper)
{
    private readonly AuthenticatedUserSteps _user = new(appDriver);

    [Fact]
    public Task AttemptToCloseAccount() => ScenarioForAuthorizedUser("close an account", _user.ClosesAnyAccount);

    [Fact]
    public Task AttemptToSetFavoriteAccount() =>
        ScenarioForAuthorizedUser("set a favorite account", _user.SetFavoriteAccount);

    [Fact]
    public Task AttemptToModifyAnAccount() => ScenarioForAuthorizedUser("modify an account", _user.ModifyAccount);

    [Fact]
    public Task AttemptToOpenAnAccount() => ScenarioForAuthorizedUser("open an account", _user.OpenAccount);

    [Fact]
    public Task AttemptToGetAccount() => ScenarioForAuthorizedUser("get an account", _user.GetAccount);

    [Fact]
    public Task AttemptToGetAccounts() => ScenarioForAuthorizedUser("get a list of accounts", _user.GetAccounts);

    [Fact]
    public Task AttemptToGetCategories() => ScenarioForAuthorizedUser("get a list of categories", _user.GetCategories);

    [Fact]
    public Task AttemptToImportData() => ScenarioForAuthorizedUser("import data", _user.ImportData);

    [Fact]
    public Task AttemptToExportData() => ScenarioForAuthorizedUser("export data", _user.ExportData);

    [Fact]
    public Task AttemptToGetImportDataStatus() =>
        ScenarioForAuthorizedUser("get import data status", _user.GetImportStatus);

    [Fact]
    public Task AttemptToGetStatisticsForAllCategories() =>
        ScenarioForAuthorizedUser("get statistics for all categories", _user.GetStatisticsForAllCategories);

    [Fact]
    public Task AttemptToGetStatisticsSummary() =>
        ScenarioForAuthorizedUser("get statistics summary", _user.GetStatisticsSummary);

    [Fact]
    public Task AttemptToGetStoreItems() =>
        ScenarioForAuthorizedUser("get store items", _user.GetStoreItems);

    [Fact]
    public Task AttemptToGetStoreItem() =>
        ScenarioForAuthorizedUser("get store item", _user.GetStoreItem);

    [Fact]
    public Task AttemptToCreateStoreItem() =>
        ScenarioForAuthorizedUser("create a store item", _user.CreateStoreItem);

    [Fact]
    public Task AttemptToModifyStoreItem() =>
        ScenarioForAuthorizedUser("modify a store item", _user.ModifyStoreItem);

    [Fact]
    public Task AttemptToGetTags() => ScenarioForAuthorizedUser("get tags", _user.GetTags);

    [Fact]
    public Task AttemptToGetCashflows() => ScenarioForAuthorizedUser("get cashflows", _user.GetCashflows);

    [Fact]
    public Task AttemptToGetCashflow() => ScenarioForAuthorizedUser("get cashflow", _user.GetCashflow);

    [Fact]
    public Task AttemptToModifyCashflow() => ScenarioForAuthorizedUser("modify cashflow", _user.ModifyCashflow);

    [Fact]
    public Task AttemptToCancelCashflow() => ScenarioForAuthorizedUser("cancel cashflow", _user.CancelCashflow);

    [Fact]
    public Task AttemptToGetUpcoming() => ScenarioForAuthorizedUser("get upcoming", _user.GetUpcoming);

    [Fact]
    public Task AttemptToMakePurchase() => ScenarioForAuthorizedUser("make purchase", _user.MakePurchase);

    [Fact]
    public Task AttemptToPayCashflow() => ScenarioForAuthorizedUser("pay cashflow", _user.PayCashflow);

    [Fact]
    public Task AttemptToTransfer() => ScenarioForAuthorizedUser("transfer", _user.Transfer);

    [Fact]
    public Task AttemptToModifyTransaction() => ScenarioForAuthorizedUser("modify transaction", _user.ModifyTransaction);

    [Fact]
    public Task AttemptToDeleteTransaction() => ScenarioForAuthorizedUser("delete transaction", _user.DeleteTransaction);

    [Fact]
    public Task AttemptToGetTransaction() => ScenarioForAuthorizedUser("get transaction", _user.GetTransaction);

    [Fact]
    public Task AttemptToGetTransactions() => ScenarioForAuthorizedUser("get transactions", _user.GetTransactions);
}
