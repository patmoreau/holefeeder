using System.Net;

using Holefeeder.Application.Models;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.GetUpcomingRequestBuilder;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;

using static Holefeeder.Application.Features.Transactions.Queries.GetUpcoming;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioGetUpcoming : BaseScenario<ScenarioGetUpcoming>
{
    public ScenarioGetUpcoming(ApiApplicationDriver apiApplicationDriver, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, testOutputHelper)
    {
        DatabaseDriver.ResetStateAsync().Wait();
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        var request = GivenAnInvalidUpcomingRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserGetsUpcoming(request);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenAuthorizedUser()
    {
        var request = GivenAnUpcomingRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserGetsUpcoming(request);

        ThenUserShouldBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenForbiddenUser()
    {
        var request = GivenAnUpcomingRequest().Build();

        GivenForbiddenUserIsAuthorized();

        await WhenUserGetsUpcoming(request);

        ShouldBeForbiddenToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnauthorizedUser()
    {
        var request = GivenAnUpcomingRequest().Build();

        GivenUserIsUnauthorized();

        await WhenUserGetsUpcoming(request);

        ShouldNotBeAuthorizedToAccessEndpoint();
    }

    [Fact]
    public async Task WhenUnpaidUpcomingOneTimeCashflow()
    {
        Cashflow cashflow = null!;
        Request request = null!;
        DateTime from = default;
        DateTime to = default;
        UpcomingViewModel[]? result = null!;

        await Given(() => User.IsAuthorized())
            .Given(async () => cashflow = await BuildCashflow(DateIntervalType.OneTime))
            .Given(() => from = cashflow.EffectiveDate.AddDays(-1))
            .Given(() => to = cashflow.EffectiveDate.AddDays(7))
            .Given(() => request = BuildUpcomingRequest(from, to))
            .When(() => WhenUserGetsUpcoming(request))
            .Then(() => ThenShouldExpectStatusCode(HttpStatusCode.OK))
            .When(() => result = HttpClientDriver.DeserializeContent<UpcomingViewModel[]>())
            .Then(() =>
            {
                result.Should().NotBeNull().And.HaveCount(1).And.BeInAscendingOrder(x => x.Date);
                result![0].Should().BeEquivalentTo(cashflow, options => options.ExcludingMissingMembers());
            })
            .RunScenarioAsync();
    }

    [Fact]
    public async Task WhenUnpaidUpcomingWeeklyCashflow()
    {
        Cashflow cashflow = null!;
        Request request = null!;
        DateTime from = default;
        DateTime to = default;
        UpcomingViewModel[]? result = null!;

        await Given(() => User.IsAuthorized())
            .Given(async () => cashflow = await BuildCashflow(DateIntervalType.Weekly, 2))
            .Given(() => from = cashflow.EffectiveDate.AddDays(-1))
            .Given(() => to = cashflow.EffectiveDate.AddDays(7*3))
            .Given(() => request = BuildUpcomingRequest(from, to))
            .When(() => WhenUserGetsUpcoming(request))
            .Then(() => ThenShouldExpectStatusCode(HttpStatusCode.OK))
            .When(() => result = HttpClientDriver.DeserializeContent<UpcomingViewModel[]>())
            .Then(() =>
            {
                result.Should().NotBeNull().And.HaveCount(3).And.BeInAscendingOrder(x => x.Date);
            })
            .RunScenarioAsync();
    }

    [Fact]
    public async Task WhenUnpaidUpcomingMonthlyCashflow()
    {
        Cashflow cashflow = null!;
        Request request = null!;
        DateTime from = default;
        DateTime to = default;
        UpcomingViewModel[]? result = null!;

        await Given(() => User.IsAuthorized())
            .Given(async () => cashflow = await BuildCashflow(DateIntervalType.Monthly))
            .Given(() => from = cashflow.EffectiveDate.AddDays(-1))
            .Given(() => to = cashflow.EffectiveDate.AddMonths(12))
            .Given(() => request = BuildUpcomingRequest(from, to))
            .When(() => WhenUserGetsUpcoming(request))
            .Then(() => ThenShouldExpectStatusCode(HttpStatusCode.OK))
            .When(() => result = HttpClientDriver.DeserializeContent<UpcomingViewModel[]>())
            .Then(() =>
            {
                result.Should().NotBeNull().And.HaveCount(12).And.BeInAscendingOrder(x => x.Date);
            })
            .RunScenarioAsync();
    }

    [Fact]
    public async Task WhenUnpaidUpcomingYearlyCashflow()
    {
        Cashflow cashflow = null!;
        Request request = null!;
        DateTime from = default;
        DateTime to = default;
        UpcomingViewModel[]? result = null!;

        await Given(() => User.IsAuthorized())
            .Given(async () => cashflow = await BuildCashflow(DateIntervalType.Yearly))
            .Given(() => from = cashflow.EffectiveDate.AddDays(-1))
            .Given(() => to = cashflow.EffectiveDate.AddYears(2))
            .Given(() => request = BuildUpcomingRequest(from, to))
            .When(() => WhenUserGetsUpcoming(request))
            .Then(() => ThenShouldExpectStatusCode(HttpStatusCode.OK))
            .When(() => result = HttpClientDriver.DeserializeContent<UpcomingViewModel[]>())
            .Then(() =>
            {
                result.Should().NotBeNull().And.HaveCount(2).And.BeInAscendingOrder(x => x.Date);
            })
            .RunScenarioAsync();
    }

    private async Task WhenUserGetsUpcoming(Request request)
    {
        await HttpClientDriver.SendGetRequest(ApiResources.GetUpcoming, request.From, request.To);
    }

    private static Request BuildUpcomingRequest(DateTime from, DateTime to) => GivenAnUpcomingRequest()
        .From(from).To(to).Build();

    private async Task<Cashflow> BuildCashflow(DateIntervalType intervalType, int frequency = 1, int recurrence = 1)
    {
        var account = await GivenAnActiveAccount()
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        var category = await GivenACategory()
            .OfType(CategoryType.Expense)
            .ForUser(AuthorizedUserId)
            .SavedInDb(DatabaseDriver);

        return await GivenAnActiveCashflow()
            .ForAccount(account)
            .ForCategory(category)
            .ForUser(AuthorizedUserId)
            .OfFrequency(intervalType, frequency)
            .Recurring(recurrence)
            .SavedInDb(DatabaseDriver);
    }
}
