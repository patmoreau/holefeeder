using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Application.Features.Transactions.Commands.PayCashflow;
using static Holefeeder.Tests.Common.Builders.Transactions.PayCashflowRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
[Collection("Api collection")]
public sealed class ScenarioPayCashflow(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task InvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.PaysACashflow)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task ValidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Cashflow.Exists)
            .And(AValidRequest)
            .When(TheUser.PaysACashflow)
            .Then(Transaction.ShouldBeCreatedFromCashflowPayment)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() => GivenAnInvalidCashflowPayment().Build());

    private static void AValidRequest(IStepRunner runner) =>
        runner.Execute<Cashflow, Request>(cashflow =>
        {
            cashflow.Should().BeValid();
            var request = GivenACashflowPayment()
                .ForCashflow(cashflow)
                .Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });
}
