using System.Globalization;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.StoreItem;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Features;

using static Holefeeder.Application.UseCases.PowerSync;
using static Holefeeder.Tests.Common.Builders.StoreItems.StoreItemBuilder;
using static Holefeeder.Tests.Common.Builders.PowerSync.SyncRequestBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.FunctionalTests.UseCases.PowerSync;

public class ScenarioSync(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.SyncTheirData)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task WhenSyncingAStoreItemDelete() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(StoreItem.Exists)
            .And(AValidStoreItemDeleteRequest)
            .When(TheUser.SyncTheirData)
            .Then(StoreItem.ShouldBeSynced)
            .PlayAsync();

    [Fact]
    public Task WhenSyncingAStoreItemPatch() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(StoreItem.Exists)
            .And(AValidStoreItemPatchRequest)
            .When(TheUser.SyncTheirData)
            .Then(StoreItem.ShouldBeSynced)
            .PlayAsync();

    [Fact]
    public Task WhenSyncingAStoreItemPut() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(StoreItem.Exists)
            .And(AValidStoreItemPutRequest)
            .When(TheUser.SyncTheirData)
            .Then(StoreItem.ShouldBeSynced)
            .PlayAsync();

    [Fact]
    public Task WhenSyncingATransactionDelete() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Transaction.Exists)
            .And(AValidTransactionDeleteRequest)
            .When(TheUser.SyncTheirData)
            .Then(Transaction.ShouldBeSynced)
            .PlayAsync();

    [Fact]
    public Task WhenSyncingATransactionPatch() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Transaction.Exists)
            .And(AValidTransactionPatchRequest)
            .When(TheUser.SyncTheirData)
            .Then(Transaction.ShouldBeSynced)
            .PlayAsync();

    [Fact]
    public Task WhenSyncingATransactionPut() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(AValidTransactionPutRequest)
            .When(TheUser.SyncTheirData)
            .Then(Transaction.ShouldBeSynced)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() =>
        {
            var request = GivenASyncRequest()
                .WithNoTransactionId()
                .Build();

            return request;
        });

    private static void AValidStoreItemDeleteRequest(IStepRunner runner) =>
        runner.Execute<StoreItem, Request>(storeItem =>
        {
            storeItem.Should().BeValid();
            var request = GivenASyncRequest()
                .WithType("store_items")
                .WithOperation("DELETE")
                .WithId(storeItem.Value.Id)
                .Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    private static void AValidStoreItemPatchRequest(IStepRunner runner) =>
        runner.Execute<StoreItem, Request>(storeItem =>
        {
            storeItem.Should().BeValid();
            var modified = storeItem.Value.Modify(data: "Updated via sync");
            runner.SetContextData(PowerSyncContext.SyncData, modified.Value);
            var request = GivenASyncRequest()
                .WithType("store_items")
                .WithOperation("PATCH")
                .WithId(storeItem.Value.Id)
                .WithData(new Dictionary<string, object>
                {
                    {"data", modified.Value.Data}
                })
                .Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    private static void AValidStoreItemPutRequest(IStepRunner runner) =>
        runner.Execute(() =>
        {
            var storeItem = GivenAStoreItem()
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .Build();

            runner.SetContextData(PowerSyncContext.SyncData, storeItem);
            var request = GivenASyncRequest()
                .WithType("store_items")
                .WithOperation("PUT")
                .WithId(storeItem.Id)
                .WithData(new Dictionary<string, object>
                {
                    {"code", storeItem.Code},
                    {"data", storeItem.Data},
                })
                .Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    private static void AValidTransactionDeleteRequest(IStepRunner runner) =>
        runner.Execute<Transaction, Request>(transaction =>
        {
            transaction.Should().BeValid();
            var request = GivenASyncRequest()
                .WithType("transactions")
                .WithOperation("DELETE")
                .WithId(transaction.Value.Id)
                .Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    private static void AValidTransactionPatchRequest(IStepRunner runner) =>
        runner.Execute<Transaction, Request>(transaction =>
        {
            transaction.Should().BeValid();
            var modified = transaction.Value.Modify(amount: transaction.Value.Amount * 2m, description: "Updated via sync");
            runner.SetContextData(PowerSyncContext.SyncData, modified.Value);
            var request = GivenASyncRequest()
                .WithType("transactions")
                .WithOperation("PATCH")
                .WithId(transaction.Value.Id)
                .WithData(new Dictionary<string, object>
                {
                    {"amount", (int) (modified.Value.Amount * 100m)}, // Amount is in cents
                    {"description", modified.Value.Description}
                })
                .Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    private static void AValidTransactionPutRequest(IStepRunner runner) =>
        runner.Execute(() =>
        {
            var account = runner.GetContextData<Account>(AccountContext.ExistingAccount);
            account.Should().NotBeNull();
            var category = runner.GetContextData<Category>(CategoryContext.ExistingCategory);
            account.Should().NotBeNull();
            category.Should().NotBeNull();

            var transaction = GivenATransaction()
                .ForAccount(account)
                .ForCategory(category)
                .ForUser(TestUsers[AuthorizedUser].UserId)
                .Build();

            runner.SetContextData(PowerSyncContext.SyncData, transaction);
            var request = GivenASyncRequest()
                .WithType("transactions")
                .WithOperation("PUT")
                .WithId(transaction.Id)
                .WithData(new Dictionary<string, object>
                {
                    {"date", transaction.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}, // Amount is in cents
                    {"amount", (int) (transaction.Amount * 100m)}, // Amount is in cents
                    {"description", transaction.Description},
                    {"account_id", transaction.AccountId},
                    {"category_id", transaction.CategoryId},
                    {"tags", string.Join(",", transaction.Tags)},
                })
                .Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });
}
