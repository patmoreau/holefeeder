using Bogus;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.Testing.Drivers;
using DrifterApps.Seeds.Testing.StepDefinitions;

using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Tests.Common.Builders.Accounts.CloseAccountRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Accounts.FavoriteAccountRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Accounts.ModifyAccountRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Accounts.OpenAccountRequestBuilder;
using static Holefeeder.Tests.Common.Builders.MyData.ImportDataRequestBuilder;
using static Holefeeder.Tests.Common.Builders.StoreItems.CreateStoreItemRequestBuilder;
using static Holefeeder.Tests.Common.Builders.StoreItems.ModifyStoreItemRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CancelCashflowRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.DeleteTransactionRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.GetUpcomingRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.MakePurchaseRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.ModifyCashflowRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.ModifyTransactionRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.PayCashflowRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransferRequestBuilder;

namespace Holefeeder.FunctionalTests.StepDefinitions;

public abstract class AuthApiSteps<T>(IApplicationDriver applicationDriver) : ApiSteps<T>(applicationDriver)
    where T : IUser
{
    private readonly Faker _faker = new();

    internal void ClosesAnyAccount(IStepRunner runner) =>
        runner.Execute(() => Api.CloseAccountAsync(GivenACloseAccountRequest().Build()));

    internal void SetFavoriteAccount(IStepRunner runner) =>
        runner.Execute(() => Api.FavoriteAccountAsync(GivenAFavoriteAccountRequest().Build()));

    internal void ModifyAccount(IStepRunner runner) =>
        runner.Execute(() => Api.ModifyAccountAsync(GivenAModifyAccountRequest().Build()));

    internal void OpenAccount(IStepRunner runner) =>
        runner.Execute(() => Api.ModifyAccountAsync(GivenAnOpenAccountRequest().Build()));

    internal void GetAccount(IStepRunner runner) => runner.Execute(() => Api.GetAccountAsync(_faker.Random.Guid()));

    internal void GetAccounts(IStepRunner runner) => runner.Execute(() =>
        Api.GetAccountsAsync(_faker.Random.Int(0, 5), _faker.Random.Int(1, 10), _faker.Random.WordsArray(3),
            _faker.Random.WordsArray(3)));

    internal void GetCategories(IStepRunner runner) => runner.Execute(() => Api.GetCategoriesAsync());

    internal void GetAccountTypes(IStepRunner runner) => runner.Execute(() => Api.GetAccountTypesAsync());

    internal void GetCategoryTypes(IStepRunner runner) => runner.Execute(() => Api.GetCategoryTypesAsync());

    internal void GetDateIntervalTypes(IStepRunner runner) => runner.Execute(() => Api.GetDateIntervalTypesAsync());

    internal void ImportData(IStepRunner runner) =>
        runner.Execute(() => Api.ImportDataAsync(GivenAnImportDataRequest().WithNoData().Build()));

    internal void ExportData(IStepRunner runner) => runner.Execute(() => Api.ExportDataAsync());

    internal void GetImportStatus(IStepRunner runner) =>
        runner.Execute(() => Api.ImportDataStatusAsync(_faker.Random.Guid()));

    internal void GetStatisticsForAllCategories(IStepRunner runner) =>
        runner.Execute(() => Api.GetStatisticsForAllCategoriesAsync());

    internal void GetStatisticsSummary(IStepRunner runner) =>
        runner.Execute(() => Api.GetStatisticsSummaryAsync(_faker.Date.RecentDateOnly(),
            _faker.Date.SoonDateOnly()));

    internal void GetStoreItems(IStepRunner runner) =>
        runner.Execute(() => Api.GetStoreItemsAsync(string.Empty));

    internal void GetStoreItem(IStepRunner runner) =>
        runner.Execute(() => Api.GetStoreItemAsync(_faker.Random.Guid()));

    internal void CreateStoreItem(IStepRunner runner) =>
        runner.Execute(() => Api.CreateStoreItemAsync(GivenACreateStoreItemRequest().Build()));

    internal void ModifyStoreItem(IStepRunner runner) =>
        runner.Execute(() => Api.ModifyStoreItemAsync(GivenAModifyStoreItemRequest().Build()));

    internal void GetTags(IStepRunner runner) => runner.Execute(() => Api.GetTagsWithCountAsync());

    internal void GetCashflows(IStepRunner runner) => runner.Execute(() => Api.GetCashflowsAsync(string.Empty));

    internal void GetCashflow(IStepRunner runner) => runner.Execute(() => Api.GetCashflowAsync(_faker.Random.Guid()));

    internal void ModifyCashflow(IStepRunner runner) =>
        runner.Execute(() => Api.ModifyCashflowAsync(GivenAModifyCashflowRequest().Build()));

    internal void CancelCashflow(IStepRunner runner) =>
        runner.Execute(() => Api.CancelCashflowAsync(GivenACancelCashflowRequest().Build()));

    internal void GetUpcoming(IStepRunner runner) =>
        runner.Execute(() =>
        {
            var request = GivenAnUpcomingRequest().Build();
            return Api.GetUpcomingAsync(request.From, request.To);
        });

    internal void MakePurchase(IStepRunner runner) =>
        runner.Execute(() => Api.MakePurchaseAsync(GivenAPurchase().Build()));

    internal void PayCashflow(IStepRunner runner) =>
        runner.Execute(() => Api.PayCashflowAsync(GivenACashflowPayment().Build()));

    internal void Transfer(IStepRunner runner) =>
        runner.Execute(() => Api.TransferAsync(GivenATransfer().Build()));

    internal void ModifyTransaction(IStepRunner runner) =>
        runner.Execute(() => Api.ModifyTransactionAsync(GivenAModifyTransactionRequest().Build()));

    internal void DeleteTransaction(IStepRunner runner) =>
        runner.Execute(() =>
        {
            var request = GivenADeleteTransactionRequest().Build();
            return Api.DeleteTransactionAsync(request.Id);
        });

    internal void GetTransaction(IStepRunner runner) =>
        runner.Execute(() => Api.GetTransactionAsync(_faker.Random.Guid()));

    internal void GetTransactions(IStepRunner runner) =>
        runner.Execute(() => Api.GetTransactionsAsync(string.Empty));
}
