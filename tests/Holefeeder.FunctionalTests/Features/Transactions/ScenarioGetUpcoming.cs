using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

using static Holefeeder.Application.Features.Transactions.Queries.GetUpcoming;
using static Holefeeder.Tests.Common.Builders.Transactions.GetUpcomingRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class ScenarioGetUpcoming(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.GetsUpcomingCashflows)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task WhenUnpaidUpcomingOneTimeCashflow() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Cashflow.AnActiveOneTime)
            .And(ARequestForUpcomingCashflows)
            .When(TheUser.GetsUpcomingCashflows)
            .Then(ShouldReceiveUpcomingCashflows)
            .PlayAsync();

    [Fact]
    public Task WhenUnpaidUpcomingWeeklyCashflow() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Cashflow.AnActiveBiWeekly)
            .And(ARequestForUpcomingWeeklyCashflows)
            .When(TheUser.GetsUpcomingCashflows)
            .Then(ShouldReceiveUpcomingCashflows)
            .PlayAsync();

    [Fact]
    public Task WhenUnpaidUpcomingMonthlyCashflow() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Cashflow.AnActiveMonthly)
            .And(ARequestForUpcomingMonthlyCashflows)
            .When(TheUser.GetsUpcomingCashflows)
            .Then(ShouldReceiveUpcomingCashflows)
            .PlayAsync();

    [Fact]
    public Task WhenUnpaidUpcomingYearlyCashflow() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Cashflow.AnActiveYearly)
            .And(ARequestForUpcomingYearlyCashflows)
            .When(TheUser.GetsUpcomingCashflows)
            .Then(ShouldReceiveUpcomingCashflows)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() => GivenAnInvalidUpcomingRequest().Build());

    private static void ARequestForUpcomingCashflows(IStepRunner runner) =>
        runner.Execute<Cashflow, Request>(cashflow =>
        {
            cashflow.Should().BeValid();
            var request = BuildUpcomingRequest(cashflow.Value.EffectiveDate.AddDays(-1), cashflow.Value.EffectiveDate.AddDays(7));
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    private static void ARequestForUpcomingWeeklyCashflows(IStepRunner runner) =>
        runner.Execute<Cashflow, Request>(cashflow =>
        {
            cashflow.Should().BeValid();
            var request = BuildUpcomingRequest(cashflow.Value.EffectiveDate.AddDays(-1), cashflow.Value.EffectiveDate.AddDays(-1).AddDays(7 * 6));
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    private static void ARequestForUpcomingMonthlyCashflows(IStepRunner runner) =>
        runner.Execute<Cashflow, Request>(cashflow =>
        {
            cashflow.Should().BeValid();
            var request = BuildUpcomingRequest(cashflow.Value.EffectiveDate.AddDays(-1), cashflow.Value.EffectiveDate.AddDays(-1).AddMonths(12));
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    private static void ARequestForUpcomingYearlyCashflows(IStepRunner runner) =>
        runner.Execute<Cashflow, Request>(cashflow =>
        {
            cashflow.Should().BeValid();
            var request = BuildUpcomingRequest(cashflow.Value.EffectiveDate.AddDays(-1), cashflow.Value.EffectiveDate.AddDays(-1).AddYears(2));
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });

    private static void ShouldReceiveUpcomingCashflows(IStepRunner runner) =>
        runner.Execute<IApiResponse<IEnumerable<UpcomingViewModel>>>(response =>
        {
            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveContent();
            var result = response.Value.Content;
            result.Should().BeInAscendingOrder(x => x.Date);
        });

    private static Request BuildUpcomingRequest(DateOnly from, DateOnly to) => GivenAnUpcomingRequest()
        .From(from).To(to).Build();
}
