using System.Net;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;
using static Holefeeder.Tests.Common.SeedWork.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.MyData;

public class ScenarioExportData : BaseScenario
{
    public ScenarioExportData(ApiApplicationDriver applicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenDataIsExported()
    {
        var accounts = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .CollectionSavedInDb(DatabaseDriver, 2);

        var categories = await GivenACategory()
            .ForUser(AuthorizedUserId)
            .CollectionSavedInDb(DatabaseDriver, 2);

        var cashflows = await GivenAnActiveCashflow()
            .ForAccount(accounts.ElementAt(0))
            .ForCategory(categories.ElementAt(0))
            .ForUser(AuthorizedUserId)
            .CollectionSavedInDb(DatabaseDriver, 2);

        var transactions = await GivenATransaction()
            .ForAccount(accounts.ElementAt(0))
            .ForCategory(categories.ElementAt(0))
            .CollectionSavedInDb(DatabaseDriver, 2);

        GivenUserIsAuthorized();

        await WhenUserExportsHisData();

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        ExportDataDto? result = HttpClientDriver.DeserializeContent<ExportDataDto>();
        ThenAssertAll(() =>
        {
            result.Should().NotBeNull();
            AssertAccounts(result!, accounts);
            AssertCashflows(result!, cashflows);
            AssertCategories(result!, categories);
            AssertTransactions(result!, transactions);
        });

        void AssertAccounts(ExportDataDto exported, IEnumerable<Account> expected)
        {
            exported.Accounts.Should().BeEquivalentTo(expected, options => options.ExcludingMissingMembers());
        }

        void AssertCashflows(ExportDataDto exported, IEnumerable<Cashflow> expected)
        {
            exported.Cashflows.Should().BeEquivalentTo(expected, options =>
                options.ExcludingMissingMembers());
        }

        void AssertCategories(ExportDataDto exported, IEnumerable<Category> expected)
        {
            exported.Categories.Should().BeEquivalentTo(expected, options =>
                options.ExcludingMissingMembers());
        }

        void AssertTransactions(ExportDataDto exported, IEnumerable<Transaction> expected)
        {
            exported.Transactions.Should().BeEquivalentTo(expected, options =>
                options.ExcludingMissingMembers());
        }
    }

    private async Task WhenUserExportsHisData() => await HttpClientDriver.SendGetRequest(ApiResources.ExportData);
}
