using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.StepDefinitions;

namespace Holefeeder.FunctionalTests.Features.Authorization;

public class UnauthorizedScenario(ApiApplicationSecurityDriver appDriver, ITestOutputHelper testOutputHelper) :
    SecurityScenario(testOutputHelper)
{
    private readonly UnauthenticatedUserSteps _user = new(appDriver);

    [Fact]
    public Task AttemptToCloseAccount() => ScenarioForUnauthenticatedUser("close an account", _user.ClosesAnyAccount);

    [Fact]
    public Task AttemptToSetFavoriteAccount() =>
        ScenarioForUnauthenticatedUser("set a favorite account", _user.SetFavoriteAccount);

    [Fact]
    public Task AttemptToModifyAnAccount() => ScenarioForUnauthenticatedUser("modify an account", _user.ModifyAccount);

    [Fact]
    public Task AttemptToOpenAnAccount() => ScenarioForUnauthenticatedUser("open an account", _user.OpenAccount);

    [Fact]
    public Task AttemptToGetAccount() => ScenarioForUnauthenticatedUser("get an account", _user.GetAccount);

    [Fact]
    public Task AttemptToGetAccounts() => ScenarioForUnauthenticatedUser("get a list of accounts", _user.GetAccounts);

    [Fact]
    public Task AttemptToGetCategories() =>
        ScenarioForUnauthenticatedUser("get a list of categories", _user.GetCategories);

    [Fact]
    public Task AttemptToGetAccountTypes() =>
        ScenarioForAuthorizedUser("get the list of account types", _user.GetAccountTypes);

    [Fact]
    public Task AttemptToGetCategoryTypes() =>
        ScenarioForAuthorizedUser("get the list of category types", _user.GetCategoryTypes);

    [Fact]
    public Task AttemptToGetDateIntervalTypes() =>
        ScenarioForAuthorizedUser("get the list of date interval types", _user.GetDateIntervalTypes);

    [Fact]
    public Task AttemptToImportData() => ScenarioForUnauthenticatedUser("import data", _user.ImportData);

    [Fact]
    public Task AttemptToExportData() => ScenarioForUnauthenticatedUser("export data", _user.ExportData);

    [Fact]
    public Task AttemptToGetImportDataStatus() =>
        ScenarioForUnauthenticatedUser("get import data status", _user.GetImportStatus);

    [Fact]
    public Task AttemptToGetStatisticsForAllCategories() =>
        ScenarioForUnauthenticatedUser("get statistics for all categories", _user.GetStatisticsForAllCategories);

    [Fact]
    public Task AttemptToGetStatisticsSummary() =>
        ScenarioForUnauthenticatedUser("get statistics summary", _user.GetStatisticsSummary);

    [Fact]
    public Task AttemptToGetStoreItems() =>
        ScenarioForUnauthenticatedUser("get store items", _user.GetStoreItems);

    [Fact]
    public Task AttemptToGetStoreItem() =>
        ScenarioForUnauthenticatedUser("get store item", _user.GetStoreItem);

    [Fact]
    public Task AttemptToCreateStoreItem() =>
        ScenarioForUnauthenticatedUser("create a store item", _user.CreateStoreItem);

    [Fact]
    public Task AttemptToModifyStoreItem() =>
        ScenarioForUnauthenticatedUser("modify a store item", _user.ModifyStoreItem);

    [Fact]
    public Task AttemptToGetTags() => ScenarioForUnauthenticatedUser("get tags", _user.GetTags);

    [Fact]
    public Task AttemptToGetCashflows() => ScenarioForUnauthenticatedUser("get cashflows", _user.GetCashflows);

    [Fact]
    public Task AttemptToGetCashflow() => ScenarioForUnauthenticatedUser("get cashflow", _user.GetCashflow);

    [Fact]
    public Task AttemptToModifyCashflow() => ScenarioForUnauthenticatedUser("modify cashflow", _user.ModifyCashflow);

    [Fact]
    public Task AttemptToCancelCashflow() => ScenarioForUnauthenticatedUser("cancel cashflow", _user.CancelCashflow);

    [Fact]
    public Task AttemptToGetUpcoming() => ScenarioForUnauthenticatedUser("get upcoming", _user.GetUpcoming);

    [Fact]
    public Task AttemptToMakePurchase() => ScenarioForUnauthenticatedUser("make purchase", _user.MakePurchase);

    [Fact]
    public Task AttemptToPayCashflow() => ScenarioForUnauthenticatedUser("pay cashflow", _user.PayCashflow);

    [Fact]
    public Task AttemptToTransfer() => ScenarioForUnauthenticatedUser("transfer", _user.Transfer);

    [Fact]
    public Task AttemptToModifyTransaction() =>
        ScenarioForUnauthenticatedUser("modify transaction", _user.ModifyTransaction);

    [Fact]
    public Task AttemptToDeleteTransaction() =>
        ScenarioForUnauthenticatedUser("delete transaction", _user.DeleteTransaction);

    [Fact]
    public Task AttemptToGetTransaction() => ScenarioForUnauthenticatedUser("get transaction", _user.GetTransaction);

    [Fact]
    public Task AttemptToGetTransactions() => ScenarioForUnauthenticatedUser("get transactions", _user.GetTransactions);
}
