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

    internal void ClosingAnAccount(IStepRunner runner) =>
        runner.Execute(() => Api.CloseAccountAsync(GivenACloseAccountRequest().Build()));

    internal void SettingAFavoriteAccount(IStepRunner runner) =>
        runner.Execute(() => Api.FavoriteAccountAsync(GivenAFavoriteAccountRequest().Build()));

    internal void ModifyingAnAccount(IStepRunner runner) =>
        runner.Execute(() => Api.ModifyAccountAsync(GivenAModifyAccountRequest().Build()));

    internal void OpeningAnAccount(IStepRunner runner) =>
        runner.Execute(() => Api.ModifyAccountAsync(GivenAnOpenAccountRequest().Build()));

    internal void GettingAnAccount(IStepRunner runner) => runner.Execute(() => Api.GetAccountAsync(_faker.Random.Guid()));

    internal void GettingAccounts(IStepRunner runner) => runner.Execute(() =>
        Api.GetAccountsAsync(_faker.Random.Int(0, 5), _faker.Random.Int(1, 10), _faker.Random.WordsArray(3),
            _faker.Random.WordsArray(3)));

    internal void GettingCategories(IStepRunner runner) => runner.Execute(() => Api.GetCategoriesAsync());

    internal void GettingAccountTypes(IStepRunner runner) => runner.Execute(() => Api.GetAccountTypesAsync());

    internal void GettingCategoryTypes(IStepRunner runner) => runner.Execute(() => Api.GetCategoryTypesAsync());

    internal void GettingDateIntervalTypes(IStepRunner runner) => runner.Execute(() => Api.GetDateIntervalTypesAsync());

    internal void ImportingData(IStepRunner runner) =>
        runner.Execute(() => Api.ImportDataAsync(GivenAnImportDataRequest().WithNoData().Build()));

    internal void ExportingData(IStepRunner runner) => runner.Execute(() => Api.ExportDataAsync());

    internal void GettingImportStatus(IStepRunner runner) =>
        runner.Execute(() => Api.ImportDataStatusAsync(_faker.Random.Guid()));

    internal void ComputePeriod(IStepRunner runner) =>
        runner.Execute(() => Api.ComputePeriod(_faker.Date.SoonDateOnly(), _faker.Random.Int(min: 1, max: 10)));

    internal void GettingStatisticsForAllCategories(IStepRunner runner) =>
        runner.Execute(() => Api.GetStatisticsForAllCategoriesAsync());

    internal void GettingStatisticsSummary(IStepRunner runner) =>
        runner.Execute(() => Api.GetStatisticsSummaryAsync(_faker.Date.RecentDateOnly(),
            _faker.Date.SoonDateOnly()));

    internal void GettingItemsFromTheStore(IStepRunner runner) =>
        runner.Execute(() => Api.GetStoreItemsAsync(_faker.Random.Int(0, 5), _faker.Random.Int(1, 10),
            _faker.Random.WordsArray(3), _faker.Random.WordsArray(3)));

    internal void GettingAnItemFromTheStore(IStepRunner runner) =>
        runner.Execute(() => Api.GetStoreItemAsync(_faker.Random.Guid()));

    internal void CreatingAnItemInTheStore(IStepRunner runner) =>
        runner.Execute(() => Api.CreateStoreItemAsync(GivenACreateStoreItemRequest().Build()));

    internal void ModifyingAnItemInTheStore(IStepRunner runner) =>
        runner.Execute(() => Api.ModifyStoreItemAsync(GivenAModifyStoreItemRequest().Build()));

    internal void GettingTags(IStepRunner runner) => runner.Execute(() => Api.GetTagsWithCountAsync());

    internal void GettingCashflows(IStepRunner runner) => runner.Execute(() => Api.GetCashflowsAsync(
        _faker.Random.Int(0, 5), _faker.Random.Int(1, 10), _faker.Random.WordsArray(3), _faker.Random.WordsArray(3)));

    internal void GettingACashflow(IStepRunner runner) => runner.Execute(() => Api.GetCashflowAsync(_faker.Random.Guid()));

    internal void ModifyingACashflow(IStepRunner runner) =>
        runner.Execute(() => Api.ModifyCashflowAsync(GivenAModifyCashflowRequest().Build()));

    internal void CancellingACashflow(IStepRunner runner) =>
        runner.Execute(() => Api.CancelCashflowAsync(GivenACancelCashflowRequest().Build()));

    internal void GettingUpcomingCashflows(IStepRunner runner) =>
        runner.Execute(() =>
        {
            var request = GivenAnUpcomingRequest().Build();
            return Api.GetUpcomingAsync(request.From, request.To, request.AccountId);
        });

    internal void MakingAPurchase(IStepRunner runner) =>
        runner.Execute(() => Api.MakePurchaseAsync(GivenAPurchase().Build()));

    internal void PayingACashflow(IStepRunner runner) =>
        runner.Execute(() => Api.PayCashflowAsync(GivenACashflowPayment().Build()));

    internal void TransferringMoneyBetweenTwoAccounts(IStepRunner runner) =>
        runner.Execute(() => Api.TransferAsync(GivenATransfer().Build()));

    internal void ModifyingATransaction(IStepRunner runner) =>
        runner.Execute(() => Api.ModifyTransactionAsync(GivenAModifyTransactionRequest().Build()));

    internal void DeletingATransaction(IStepRunner runner) =>
        runner.Execute(() =>
        {
            var request = GivenADeleteTransactionRequest().Build();
            return Api.DeleteTransactionAsync(request.Id);
        });

    internal void GettingATransaction(IStepRunner runner) =>
        runner.Execute(() => Api.GetTransactionAsync(_faker.Random.Guid()));

    internal void GettingTransactions(IStepRunner runner) =>
        runner.Execute(() => Api.GetTransactionsAsync(_faker.Random.Int(0, 5), _faker.Random.Int(1, 10),
            _faker.Random.WordsArray(3), _faker.Random.WordsArray(3)));
}
