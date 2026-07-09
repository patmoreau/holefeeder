using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Application.Features.Transactions.Commands;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Tests.Common.Builders.Transactions.CancelCashflowRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

public class CancelCashflowScenario(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidCancelRequest)
            .When(TheUser.CancelsACashflow)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task WhenCancellingACashflow() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Cashflow.Exists)
            .And(AValidRequest)
            .When(TheUser.CancelsACashflow)
            .Then(Cashflow.ShouldBeInactive)
            .PlayAsync();

    private static void AnInvalidCancelRequest(IStepRunner runner) =>
        runner.Execute(() => GivenACancelCashflowRequest().WithNoId().Build());

    private static void AValidRequest(IStepRunner runner) =>
        runner.Execute<Cashflow, CancelCashflow.Request>(item =>
        {
            item.Should().BeValid();
            var request = GivenACancelCashflowRequest().WithId(item.Value.Id).Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });
}
