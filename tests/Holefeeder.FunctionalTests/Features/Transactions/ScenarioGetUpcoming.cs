using System.Net;

using Holefeeder.Application.Models;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.Application.Features.Transactions.Queries.GetUpcoming;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.GetUpcomingRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetUpcoming(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public async Task WhenInvalidRequest()
    {
        var request = GivenAnInvalidUpcomingRequest().Build();

        GivenUserIsAuthorized();

        await WhenUserGetsUpcoming(request);

        ShouldReceiveValidationProblemDetailsWithErrorMessage("One or more validation errors occurred.");
    }

    [Fact]
    public async Task WhenUnpaidUpcomingOneTimeCashflow()
    {
        Cashflow cashflow = null!;
        Account account = null!;
        Category category = null!;
        Request request = null!;
        UpcomingViewModel[]? result = null!;

        await ScenarioFor("unpaid upcoming one time cashflow", player =>
        {
            player
                .Given(User.IsAuthorized)
                .And("with an active one time cashflow", async () => (cashflow, account, category) = await BuildCashflow(DateIntervalType.OneTime))
                .And("who wants to get all upcoming cashflows from yesterday to the next week", () => request = BuildUpcomingRequest(cashflow.EffectiveDate.AddDays(-1), cashflow.EffectiveDate.AddDays(7)))
                .When("the request is sent", () => WhenUserGetsUpcoming(request))
                .Then("the return should be successful", () => ShouldExpectStatusCode(HttpStatusCode.OK))
                .And("the upcoming cashflow list should be received", () => result = HttpClientDriver.DeserializeContent<UpcomingViewModel[]>())
                .And("have a count of 1 sorted by date", () =>
                {
                    result.Should()
                        .NotBeNull()
                        .And.BeInAscendingOrder(x => x.Date)
                        .And.BeEquivalentTo(new[]
                        {
                            new
                            {
                                Id = (Guid)cashflow.Id,
                                Date = cashflow.EffectiveDate,
                                Amount = (decimal)cashflow.Amount,
                                cashflow.Description,
                                cashflow.Tags,
                                Category = new
                                {
                                    Id = (Guid)category.Id,
                                    category.Name,
                                    category.Type,
                                    Color = (string)category.Color
                                },
                                Account = new
                                {
                                    Id = (Guid)account.Id,
                                    account.Name
                                }
                            }
                        });
                });
        });
    }

    [Fact]
    public async Task WhenUnpaidUpcomingWeeklyCashflow()
    {
        Cashflow cashflow = null!;
        Request request = null!;
        UpcomingViewModel[]? result = null!;

        await ScenarioFor("unpaid upcoming weekly cashflows", player =>
        {
            player
                .Given(User.IsAuthorized)
                .And("with an active weekly cashflow", async () => (cashflow, _, _) = await BuildCashflow(DateIntervalType.Weekly, 2))
                .And("who wants to get all upcoming cashflows from yesterday to the next 6 weeks", () => request = BuildUpcomingRequest(cashflow.EffectiveDate.AddDays(-1), cashflow.EffectiveDate.AddDays(-1).AddDays(7 * 6)))
                .When("the request is sent", () => WhenUserGetsUpcoming(request))
                .Then("the return should be successful", () => ShouldExpectStatusCode(HttpStatusCode.OK))
                .And("the upcoming cashflow list should be received", () => result = HttpClientDriver.DeserializeContent<UpcomingViewModel[]>())
                .And("have a count of 3 sorted by date", () => result.Should().NotBeNull().And.HaveCount(3).And.BeInAscendingOrder(x => x.Date));
        });
    }

    [Fact]
    public async Task WhenUnpaidUpcomingMonthlyCashflow()
    {
        Cashflow cashflow = default!;
        Request request = default!;
        UpcomingViewModel[]? result = default!;

        await ScenarioFor("unpaid upcoming monthly cashflows", player =>
        {
            player
                .Given(User.IsAuthorized)
                .And("with an active monthly cashflow", async () => (cashflow, _, _) = await BuildCashflow(DateIntervalType.Monthly))
                .And("who wants to get all upcoming cashflows from yesterday to the next 12 months", () => request = BuildUpcomingRequest(cashflow.EffectiveDate.AddDays(-1), cashflow.EffectiveDate.AddDays(-1).AddMonths(12)))
                .When("the request is sent", () => WhenUserGetsUpcoming(request))
                .Then("the return should be successful", () => ShouldExpectStatusCode(HttpStatusCode.OK))
                .And("the upcoming cashflow list should be received", () => result = HttpClientDriver.DeserializeContent<UpcomingViewModel[]>())
                .And("have a count of 12 sorted by date", () => result.Should().NotBeNull().And.HaveCount(12).And.BeInAscendingOrder(x => x.Date));
        });
    }

    [Fact]
    public async Task WhenUnpaidUpcomingYearlyCashflow()
    {
        Cashflow cashflow = null!;
        Request request = null!;
        UpcomingViewModel[]? result = null!;

        await ScenarioFor("unpaid upcoming monthly cashflows", player =>
        {
            player
                .Given(User.IsAuthorized)
                .Given("with an active yearly cashflow", async () => (cashflow, _, _) = await BuildCashflow(DateIntervalType.Yearly))
                .Given("who wants to get all upcoming cashflows from yesterday to the next 2 years", () => request = BuildUpcomingRequest(cashflow.EffectiveDate.AddDays(-1), cashflow.EffectiveDate.AddDays(-1).AddYears(2)))
                .When("the request is sent", () => WhenUserGetsUpcoming(request))
                .Then("the return should be successful", () => ShouldExpectStatusCode(HttpStatusCode.OK))
                .And("the upcoming cashflow list should be received", () => result = HttpClientDriver.DeserializeContent<UpcomingViewModel[]>())
                .And("have a count of 2 sorted by date", () => result.Should().NotBeNull().And.HaveCount(2).And.BeInAscendingOrder(x => x.Date));
        });
    }

    private async Task WhenUserGetsUpcoming(Request request) => await HttpClientDriver.SendRequestAsync(ApiResources.GetUpcoming, request.From, request.To);

    private static Request BuildUpcomingRequest(DateOnly from, DateOnly to) => GivenAnUpcomingRequest()
        .From(from).To(to).Build();

    private async Task<(Cashflow, Account, Category)> BuildCashflow(DateIntervalType intervalType, int frequency = 1, int recurrence = 1)
    {
        var account = await GivenAnActiveAccount()
            .ForUser(TestUsers[AuthorizedUser].UserId)
            .SavedInDbAsync(DatabaseDriver);

        var category = await GivenACategory()
            .OfType(CategoryType.Expense)
            .ForUser(TestUsers[AuthorizedUser].UserId)
            .SavedInDbAsync(DatabaseDriver);

        return (await GivenAnActiveCashflow()
            .ForAccount(account)
            .ForCategory(category)
            .ForUser(TestUsers[AuthorizedUser].UserId)
            .OfFrequency(intervalType, frequency)
            .Recurring(recurrence)
            .SavedInDbAsync(DatabaseDriver), account, category);
    }
}
