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

public class ScenarioGetUpcoming : BaseScenario
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
    public void WhenUnpaidUpcomingOneTimeCashflow()
    {
        ScenarioFor("unpaid upcoming one time cashflow", player =>
        {
            Cashflow cashflow = null!;
            Request request = null!;
            UpcomingViewModel[]? result = null!;

            player
                .Given("an authorized user", () => User.IsAuthorized())
                .And("with an active one time cashflow", async () => cashflow = await BuildCashflow(DateIntervalType.OneTime))
                .And("who wants to get all upcoming cashflows from yesterday to the next week", () => request = BuildUpcomingRequest(cashflow.EffectiveDate.AddDays(-1), cashflow.EffectiveDate.AddDays(7)))
                .When("the request is sent", () => WhenUserGetsUpcoming(request))
                .Then("the return should be successful", () => ThenShouldExpectStatusCode(HttpStatusCode.OK))
                .And("the upcoming cashflow list should be received", () => result = HttpClientDriver.DeserializeContent<UpcomingViewModel[]>())
                .And("have a count of 1 sorted by date", () =>
                {
                    result.Should().NotBeNull().And.HaveCount(1).And.BeInAscendingOrder(x => x.Date);
                    result![0].Should().BeEquivalentTo(cashflow, options => options.ExcludingMissingMembers());
                });
        });
    }

    [Fact]
    public void WhenUnpaidUpcomingWeeklyCashflow()
    {
        ScenarioFor("unpaid upcoming weekly cashflows", player =>
        {
            Cashflow cashflow = null!;
            Request request = null!;
            UpcomingViewModel[]? result = null!;

            player
                .Given("an authorized user", () => User.IsAuthorized())
                .And("with an active weekly cashflow", async () => cashflow = await BuildCashflow(DateIntervalType.Weekly, 2))
                .And("who wants to get all upcoming cashflows from yesterday to the next 3 weeks", () => request = BuildUpcomingRequest(cashflow.EffectiveDate.AddDays(-1), cashflow.EffectiveDate.AddDays(7 * 3)))
                .When("the request is sent", () => WhenUserGetsUpcoming(request))
                .Then("the return should be successful", () => ThenShouldExpectStatusCode(HttpStatusCode.OK))
                .And("the upcoming cashflow list should be received", () => result = HttpClientDriver.DeserializeContent<UpcomingViewModel[]>())
                .And("have a count of 3 sorted by date", () => result.Should().NotBeNull().And.HaveCount(3).And.BeInAscendingOrder(x => x.Date));
        });
    }

    [Fact]
    public void WhenUnpaidUpcomingMonthlyCashflow()
    {
        ScenarioFor("unpaid upcoming monthly cashflows", player =>
        {
            Cashflow cashflow = default!;
            Request request = default!;
            UpcomingViewModel[]? result = default!;

            player
                .Given("an authorized user", () => User.IsAuthorized())
                .And("with an active monthly cashflow", async () => cashflow = await BuildCashflow(DateIntervalType.Monthly))
                .And("who wants to get all upcoming cashflows from yesterday to the next 12 months", () => request = BuildUpcomingRequest(cashflow.EffectiveDate.AddDays(-1), cashflow.EffectiveDate.AddMonths(12)))
                .When("the request is sent", () => WhenUserGetsUpcoming(request))
                .Then("the return should be successful", () => ThenShouldExpectStatusCode(HttpStatusCode.OK))
                .And("the upcoming cashflow list should be received", () => result = HttpClientDriver.DeserializeContent<UpcomingViewModel[]>())
                .And("have a count of 12 sorted by date", () => result.Should().NotBeNull().And.HaveCount(12).And.BeInAscendingOrder(x => x.Date));
        });
    }

    [Fact]
    public void WhenUnpaidUpcomingYearlyCashflow()
    {
        ScenarioFor("unpaid upcoming monthly cashflows", player =>
        {
            Cashflow cashflow = null!;
            Request request = null!;
            UpcomingViewModel[]? result = null!;

            player
                .Given("an authorized user", () => User.IsAuthorized())
                .Given("with an active yearly cashflow", async () => cashflow = await BuildCashflow(DateIntervalType.Yearly))
                .Given("who wants to get all upcoming cashflows from yesterday to the next 2 years", () => request = BuildUpcomingRequest(cashflow.EffectiveDate.AddDays(-1), cashflow.EffectiveDate.AddYears(2)))
                .When("the request is sent", () => WhenUserGetsUpcoming(request))
                .Then("the return should be successful", () => ThenShouldExpectStatusCode(HttpStatusCode.OK))
                .And("the upcoming cashflow list should be received", () => result = HttpClientDriver.DeserializeContent<UpcomingViewModel[]>())
                .And("have a count of 2 sorted by date", () => result.Should().NotBeNull().And.HaveCount(2).And.BeInAscendingOrder(x => x.Date));
        });
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
