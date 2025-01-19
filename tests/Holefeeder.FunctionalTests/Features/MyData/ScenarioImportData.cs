using System.Net;

using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.Testing.Extensions;

using Holefeeder.Application;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

using static Holefeeder.Tests.Common.Builders.MyData.ImportDataRequestBuilder;
using static Holefeeder.Tests.Common.Builders.MyData.MyDataAccountDtoBuilder;
using static Holefeeder.Tests.Common.Builders.MyData.MyDataCashflowDtoBuilder;
using static Holefeeder.Tests.Common.Builders.MyData.MyDataCategoryDtoBuilder;
using static Holefeeder.Tests.Common.Builders.MyData.MyDataTransactionDtoBuilder;

namespace Holefeeder.FunctionalTests.Features.MyData;

public class ScenarioImportData(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.ImportsTheirData)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task WhenDataIsImported() =>
    ScenarioRunner.Create(ScenarioOutput)
        .Given(TheUserOwnsData)
        .When(TheUser.ImportsTheirData)
        .Then(TheImportCommandIdIsReceived)
        .When(TheUser.WaitsForImportCompletion)
        .Then(TheDataImportHasCompleted)
        .And(TheAccountsHaveBeenImported)
        .And(TheCategoriesHaveBeenImported)
        .And(TheCashflowsHaveBeenImported)
        .And(TheTransactionsHaveBeenImported)
        .PlayAsync();

    private static void TheUserOwnsData(IStepRunner runner) =>
        runner.Execute(() =>
        {
            var accounts = GivenMyAccountData().BuildCollection(2);
            runner.SetContextData(AccountContext.ExistingAccounts, accounts);

            var categories = GivenMyCategoryData().BuildCollection(2);
            runner.SetContextData(CategoryContext.ExistingCategories, categories);

            var cashflows = GivenMyCashflowData()
                .WithAccount(accounts.ElementAt(0))
                .WithCategory(categories.ElementAt(0))
                .BuildCollection(2);
            runner.SetContextData(CashflowContext.ExistingCashflows, cashflows);

            var transactions = GivenMyTransactionData()
                .WithAccount(accounts.ElementAt(0))
                .WithCategory(categories.ElementAt(0))
                .BuildCollection(2);
            runner.SetContextData(TransactionContext.ExistingTransactions, transactions);

            var request = GivenAnImportDataRequest()
                .WithUpdateExisting()
                .WithAccounts(accounts.ToArray())
                .WithCategories(categories.ToArray())
                .WithCashflows(cashflows.ToArray())
                .WithTransactions(transactions.ToArray())
                .Build();

            return request;
        });

    private static void TheImportCommandIdIsReceived(IStepRunner runner) =>
        runner.Execute<IApiResponse, Guid>(response =>
        {
            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveStatusCode(HttpStatusCode.Accepted)
                .And.HaveLocation();

            var id = response.Value.Headers.Location!.ExtractGuidFromUrl();

            return id;
        });

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() =>
        {
            var request = GivenAnImportDataRequest()
                .WithNoData()
                .Build();

            return request;
        });

    private static void TheDataImportHasCompleted(IStepRunner runner) =>
        runner.Execute<ImportDataStatusDto?>(importStatus =>
        {
            importStatus.Should().BeValid()
                .And.Subject.Value.Should().NotBeNull("The import task never completed");
            importStatus.Value!.Status.Should().NotBe(CommandStatus.Error, importStatus.Value.Message);
        });

    private void TheAccountsHaveBeenImported(IStepRunner runner) =>
        runner.Execute(async () =>
        {
            var accounts = runner.GetContextData<IReadOnlyCollection<MyDataAccountDto>>(AccountContext.ExistingAccounts);
            await AssertAccount(accounts.ElementAt(0));
            await AssertAccount(accounts.ElementAt(1));
        });

    private void TheCategoriesHaveBeenImported(IStepRunner runner) =>
        runner.Execute(async () =>
        {
            var categories = runner.GetContextData<IReadOnlyCollection<MyDataCategoryDto>>(CategoryContext.ExistingCategories);
            await AssertCategory(categories.ElementAt(0));
            await AssertCategory(categories.ElementAt(1));
        });

    private void TheCashflowsHaveBeenImported(IStepRunner runner) =>
        runner.Execute(async () =>
        {
            var cashflows = runner.GetContextData<IReadOnlyCollection<MyDataCashflowDto>>(CashflowContext.ExistingCashflows);
            await AssertCashflow(cashflows.ElementAt(0));
            await AssertCashflow(cashflows.ElementAt(1));
        });

    private void TheTransactionsHaveBeenImported(IStepRunner runner) =>
        runner.Execute(async () =>
        {
            var transactions = runner.GetContextData<IReadOnlyCollection<MyDataTransactionDto>>(TransactionContext.ExistingTransactions);
            await AssertTransaction(transactions.ElementAt(0));
            await AssertTransaction(transactions.ElementAt(1));
        });

    private async Task AssertAccount(MyDataAccountDto account)
    {
        await using var dbContext = DatabaseDriver.CreateDbContext();

        var result = await dbContext.Accounts.FindAsync((AccountId)account.Id);
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(account);
    }

    private async Task AssertCategory(MyDataCategoryDto category)
    {
        await using var dbContext = DatabaseDriver.CreateDbContext();

        var result = await dbContext.Categories.FindAsync((CategoryId)category.Id);
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(category);
    }

    private async Task AssertCashflow(MyDataCashflowDto cashflow)
    {
        await using var dbContext = DatabaseDriver.CreateDbContext();

        var result = await dbContext.Cashflows.FindAsync((CashflowId)cashflow.Id);
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(cashflow);
    }

    private async Task AssertTransaction(MyDataTransactionDto transaction)
    {
        await using var dbContext = DatabaseDriver.CreateDbContext();

        var result = await dbContext.Transactions.FindAsync((TransactionId)transaction.Id);
        result.Should().NotBeNull();
        result!.Should().BeEquivalentTo(transaction);
    }
}
