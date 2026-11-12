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
using static Holefeeder.Tests.Common.Builders.PowerSync.SyncRequestBuilder;
using static Holefeeder.Tests.Common.Builders.StoreItems.CreateStoreItemRequestBuilder;
using static Holefeeder.Tests.Common.Builders.StoreItems.ModifyStoreItemRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.DeleteTransactionRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.ModifyCashflowRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.ModifyTransactionRequestBuilder;

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

    internal void GettingTheCurrentUser(IStepRunner runner) => runner.Execute(() => Api.GetCurrentUserAsync());

    internal void RegisteringTheUser(IStepRunner runner) => runner.Execute(() => Api.RegisterUserAsync());

    internal void ImportingData(IStepRunner runner) =>
        runner.Execute(() => Api.ImportDataAsync(GivenAnImportDataRequest().WithNoData().Build()));

    internal void ExportingData(IStepRunner runner) => runner.Execute(() => Api.ExportDataAsync());

    internal void GettingImportStatus(IStepRunner runner) =>
        runner.Execute(() => Api.ImportDataStatusAsync(_faker.Random.Guid()));

    internal void GettingDashboardSummary(IStepRunner runner) =>
        runner.Execute(() => Api.GetDashboardSummary());

    internal void GettingItemsFromTheStore(IStepRunner runner) =>
        runner.Execute(() => Api.GetStoreItemsAsync(_faker.Random.Int(0, 5), _faker.Random.Int(1, 10),
            _faker.Random.WordsArray(3), _faker.Random.WordsArray(3)));

    internal void GettingAnItemFromTheStore(IStepRunner runner) =>
        runner.Execute(() => Api.GetStoreItemAsync(_faker.Random.Guid()));

    internal void CreatingAnItemInTheStore(IStepRunner runner) =>
        runner.Execute(() => Api.CreateStoreItemAsync(GivenACreateStoreItemRequest().Build()));

    internal void ModifyingAnItemInTheStore(IStepRunner runner) =>
        runner.Execute(() => Api.ModifyStoreItemAsync(GivenAModifyStoreItemRequest().Build()));

    internal void ModifyingACashflow(IStepRunner runner) =>
        runner.Execute(() => Api.ModifyCashflowAsync(GivenAModifyCashflowRequest().Build()));

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

    internal void Sync(IStepRunner runner) =>
        runner.Execute(() => Api.SyncAsync(GivenASyncRequest().Build()));
}
