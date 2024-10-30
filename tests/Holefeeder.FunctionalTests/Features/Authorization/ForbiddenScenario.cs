using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;

namespace Holefeeder.FunctionalTests.Features.Authorization;

public class ForbiddenScenario(ApiApplicationSecurityDriver appDriver, ITestOutputHelper testOutputHelper) :
    SecurityScenario(testOutputHelper)
{
    private readonly ForbiddenUserSteps _user = new(appDriver);

    [Fact]
    public Task AttemptToCloseAccount() => ScenarioForForbiddenUser("close an account", _user.ClosesAnyAccount);

    [Fact]
    public Task AttemptToSetFavoriteAccount() =>
        ScenarioForForbiddenUser("set a favorite account", _user.SetFavoriteAccount);

    [Fact]
    public Task AttemptToModifyAnAccount() => ScenarioForForbiddenUser("modify an account", _user.ModifyAccount);

    [Fact]
    public Task AttemptToOpenAnAccount() => ScenarioForForbiddenUser("open an account", _user.OpenAccount);

    [Fact]
    public Task AttemptToGetAccount() => ScenarioForForbiddenUser("get an account", _user.GetAccount);

    [Fact]
    public Task AttemptToGetAccounts() => ScenarioForForbiddenUser("get a list of accounts", _user.GetAccounts);

    [Fact]
    public Task AttemptToGetCategories() => ScenarioForForbiddenUser("get a list of categories", _user.GetCategories);

    [Fact]
    public Task AttemptToImportData() => ScenarioForForbiddenUser("import data", _user.ImportData);

    [Fact]
    public Task AttemptToExportData() => ScenarioForForbiddenUser("export data", _user.ExportData);

    [Fact]
    public Task AttemptToGetImportDataStatus() =>
        ScenarioForForbiddenUser("get import data status", _user.GetImportStatus);

    [Fact]
    public Task AttemptToGetStatisticsForAllCategories() =>
        ScenarioForForbiddenUser("get statistics for all categories", _user.GetStatisticsForAllCategories);

    [Fact]
    public Task AttemptToGetStatisticsSummary() =>
        ScenarioForForbiddenUser("get statistics summary", _user.GetStatisticsSummary);

    [Fact]
    public Task AttemptToGetStoreItems() =>
        ScenarioForForbiddenUser("get store items", _user.GetStoreItems);

    [Fact]
    public Task AttemptToGetStoreItem() =>
        ScenarioForForbiddenUser("get store item", _user.GetStoreItem);

    [Fact]
    public Task AttemptToCreateStoreItem() =>
        ScenarioForForbiddenUser("create a store item", _user.CreateStoreItem);

    [Fact]
    public Task AttemptToModifyStoreItem() =>
        ScenarioForForbiddenUser("modify a store item", _user.ModifyStoreItem);

    [Fact]
    public Task AttemptToGetTags() => ScenarioForForbiddenUser("get tags", _user.GetTags);

    [Fact]
    public Task AttemptToGetCashflows() => ScenarioForForbiddenUser("get cashflows", _user.GetCashflows);

    [Fact]
    public Task AttemptToGetCashflow() => ScenarioForForbiddenUser("get cashflow", _user.GetCashflow);

    [Fact]
    public Task AttemptToModifyCashflow() => ScenarioForForbiddenUser("modify cashflow", _user.ModifyCashflow);

    [Fact]
    public Task AttemptToCancelCashflow() => ScenarioForForbiddenUser("cancel cashflow", _user.CancelCashflow);

    [Fact]
    public Task AttemptToGetUpcoming() => ScenarioForForbiddenUser("get upcoming", _user.GetUpcoming);

    [Fact]
    public Task AttemptToMakePurchase() => ScenarioForForbiddenUser("make purchase", _user.MakePurchase);

    [Fact]
    public Task AttemptToPayCashflow() => ScenarioForForbiddenUser("pay cashflow", _user.PayCashflow);

    [Fact]
    public Task AttemptToTransfer() => ScenarioForForbiddenUser("transfer", _user.Transfer);

    [Fact]
    public Task AttemptToModifyTransaction() => ScenarioForForbiddenUser("modify transaction", _user.ModifyTransaction);

    [Fact]
    public Task AttemptToDeleteTransaction() => ScenarioForForbiddenUser("delete transaction", _user.DeleteTransaction);

    [Fact]
    public Task AttemptToGetTransaction() => ScenarioForForbiddenUser("get transaction", _user.GetTransaction);

    [Fact]
    public Task AttemptToGetTransactions() => ScenarioForForbiddenUser("get transactions", _user.GetTransactions);
}
