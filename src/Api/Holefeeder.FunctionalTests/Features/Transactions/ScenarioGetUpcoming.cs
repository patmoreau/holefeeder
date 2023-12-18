using System.Net;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Enumerations;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.Application.Features.Transactions.Queries.GetUpcoming;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Accounts.AccountBuilder;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.CashflowBuilder;
using static Holefeeder.Tests.Common.Builders.Transactions.GetUpcomingRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetUpcoming : HolefeederScenario
{
    public ScenarioGetUpcoming(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenInvalidRequest()
    {
        Request request = GivenAnInvalidUpcomingRequest().Build();

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
                                cashflow.Id,
                                Date = cashflow.EffectiveDate,
                                cashflow.Amount,
                                cashflow.Description,
                                cashflow.Tags,
                                Category = new
                                {
                                    category.Id,
                                    category.Name,
                                    category.Type,
                                    category.Color
                                },
                                Account = new
                                {
                                    account.Id,
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

    private async Task WhenUserGetsUpcoming(Request request) => await HttpClientDriver.SendGetRequestAsync(ApiResources.GetUpcoming, request.From, request.To);

    private static Request BuildUpcomingRequest(DateOnly from, DateOnly to) => GivenAnUpcomingRequest()
        .From(from).To(to).Build();

    private async Task<(Cashflow, Account, Category)> BuildCashflow(DateIntervalType intervalType, int frequency = 1, int recurrence = 1)
    {
        Account account = await GivenAnActiveAccount()
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        Category category = await GivenACategory()
            .OfType(CategoryType.Expense)
            .ForUser(HolefeederUserId)
            .SavedInDbAsync(DatabaseDriver);

        return (await GivenAnActiveCashflow()
            .ForAccount(account)
            .ForCategory(category)
            .ForUser(HolefeederUserId)
            .OfFrequency(intervalType, frequency)
            .Recurring(recurrence)
            .SavedInDbAsync(DatabaseDriver), account, category);
    }
}
