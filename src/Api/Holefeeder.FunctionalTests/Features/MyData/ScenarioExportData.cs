using System.Net;

using FluentAssertions;

using Holefeeder.Application.Features.MyData.Models;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using Holefeeder.Infrastructure.Entities;

using Xunit;
using Xunit.Abstractions;

using static Holefeeder.Tests.Common.Builders.AccountEntityBuilder;
using static Holefeeder.Tests.Common.Builders.CashflowEntityBuilder;
using static Holefeeder.Tests.Common.Builders.CategoryEntityBuilder;
using static Holefeeder.Tests.Common.Builders.TransactionEntityBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

namespace Holefeeder.FunctionalTests.Features.MyData;

public class ScenarioExportData : BaseScenario
{
    private readonly HolefeederDatabaseDriver _holefeederDatabaseDriver;

    public ScenarioExportData(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        _holefeederDatabaseDriver = apiApplicationDriver.CreateHolefeederDatabaseDriver();
        _holefeederDatabaseDriver.ResetStateAsync().Wait();
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
        var accounts = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .CollectionSavedInDb(_holefeederDatabaseDriver, 2);

        var categories = await GivenACategory()
            .ForUser(AuthorizedUserId)
            .CollectionSavedInDb(_holefeederDatabaseDriver, 2);

        var cashflows = await GivenACashflow()
            .ForAccount(accounts[0])
            .ForCategory(categories[0])
            .CollectionSavedInDb(_holefeederDatabaseDriver, 2);

        var transactions = await GivenATransaction()
            .ForAccount(accounts[0])
            .ForCategory(categories[0])
            .CollectionSavedInDb(_holefeederDatabaseDriver, 2);

        GivenUserIsAuthorized();

        await WhenUserExportsHisData();

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<ExportDataDto>();
        ThenAssertAll(() =>
        {
            result.Should().NotBeNull();
            AssertAccounts(result!, accounts);
            AssertCashflows(result!, cashflows);
            AssertCategories(result!, categories);
            AssertTransactions(result!, transactions);
        });

        void AssertAccounts(ExportDataDto exported, IEnumerable<AccountEntity> expected)
        {
            exported.Accounts.Should().BeEquivalentTo(expected, options => options.Excluding(info => info.UserId));
        }

        void AssertCashflows(ExportDataDto exported, IEnumerable<CashflowEntity> expected)
        {
            exported.Cashflows.Should().BeEquivalentTo(expected, options =>
                options
                    .Excluding(info => info.UserId)
                    .Excluding(info => info.LastPaidDate)
                    .Excluding(info => info.LastCashflowDate)
                    .Excluding(info => info.Account)
                    .Excluding(info => info.Category)
                    .Using<object>(ctx => string.Join(',', (string[])ctx.Subject).Should().Be((string)ctx.Expectation))
                    .When(info => info.Path.EndsWith("Tags")));
        }

        void AssertCategories(ExportDataDto exported, IEnumerable<CategoryEntity> expected)
        {
            exported.Categories.Should().BeEquivalentTo(expected, options => options.Excluding(info => info.UserId));
        }

        void AssertTransactions(ExportDataDto exported, IEnumerable<TransactionEntity> expected)
        {
            exported.Transactions.Should().BeEquivalentTo(expected, options =>
                options
                    .Excluding(info => info.UserId)
                    .Excluding(info => info.Account)
                    .Excluding(info => info.Category)
                    .Using<object>(ctx => string.Join(',', (string[])ctx.Subject).Should().Be((string)ctx.Expectation))
                    .When(info => info.Path.EndsWith("Tags")));
        }
    }

    private async Task WhenUserExportsHisData()
    {
        await HttpClientDriver.SendGetRequest(ApiResources.ExportData);
    }
}
