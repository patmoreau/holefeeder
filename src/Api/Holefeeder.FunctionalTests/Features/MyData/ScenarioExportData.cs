using System.Net;
using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.TransactionBuilder;

namespace Holefeeder.FunctionalTests.Features.MyData;

public class ScenarioExportData : BaseScenario
{
    public ScenarioExportData(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        GivenUserIsAuthorized();

        await WhenUserExportsHisData();

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        GivenForbiddenUserIsAuthorized();

        await WhenUserExportsHisData();

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        GivenUserIsUnauthorized();

        await WhenUserExportsHisData();

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenDataIsExported()
    {
        Account[] accounts = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .CollectionSavedInDb(DatabaseDriver, 2);

        Category[] categories = await GivenACategory()
            .ForUser(AuthorizedUserId)
            .CollectionSavedInDb(DatabaseDriver, 2);

        Cashflow[] cashflows = await GivenAnActiveCashflow()
            .ForAccount(accounts[0])
            .ForCategory(categories[0])
            .ForUser(AuthorizedUserId)
            .CollectionSavedInDb(DatabaseDriver, 2);

        Transaction[] transactions = await GivenATransaction()
            .ForAccount(accounts[0])
            .ForCategory(categories[0])
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
