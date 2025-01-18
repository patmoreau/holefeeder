using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

namespace Holefeeder.FunctionalTests.Features.MyData;

public class ScenarioExportData(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenUserExportsTheirData() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(TheData.TheUserHasFullSetsOfData)
            .When(TheUser.ExportsTheirData)
            .Then(TheUserDataIsExported)
            .PlayAsync();

    private static void TheUserDataIsExported(IStepRunner runner) =>
        runner.Execute<IApiResponse<ExportDataDto>>(response =>
        {
            response.Should().BeValid()
                .And.Subject
                .Value.Should().BeSuccessful();

            var result = response.Value.Content;
            result.Should().NotBeNull();

            var accounts = runner.GetContextData<IReadOnlyCollection<Account>>(AccountContext.ExistingAccounts);
            var cashflows = runner.GetContextData<IReadOnlyCollection<Cashflow>>(CashflowContext.ExistingCashflows);
            var categories = runner.GetContextData<IReadOnlyCollection<Category>>(CategoryContext.ExistingCategory);
            var transactions = runner.GetContextData<IReadOnlyCollection<Transaction>>(TransactionContext.ExistingTransactions);
            AssertAccounts(result!, accounts);
            AssertCashflows(result!, cashflows);
            AssertCategories(result!, categories);
            AssertTransactions(result!, transactions);
        });

    private static void AssertAccounts(ExportDataDto exported, IEnumerable<Account> expected) =>
        exported.Accounts.Should().BeEquivalentTo(expected, options => options.ExcludingMissingMembers());

    private static void AssertCashflows(ExportDataDto exported, IEnumerable<Cashflow> expected) =>
        exported.Cashflows.Should().BeEquivalentTo(expected, options => options.ExcludingMissingMembers());

    private static void AssertCategories(ExportDataDto exported, IEnumerable<Category> expected) =>
        exported.Categories.Should().BeEquivalentTo(expected, options => options.ExcludingMissingMembers());

    private static void AssertTransactions(ExportDataDto exported, IEnumerable<Transaction> expected) =>
        exported.Transactions.Should().BeEquivalentTo(expected, options => options.ExcludingMissingMembers());
}
