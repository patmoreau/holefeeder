using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Application.Features.Transactions.Commands;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Tests.Common.Builders.Transactions.ModifyCashflowRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioModifyCashflow(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.ModifiesACashflow)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task WhenModifyACashflow() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Cashflow.Exists)
            .And(AValidRequest)
            .When(TheUser.ModifiesACashflow)
            .Then(Cashflow.ShouldBeModified)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() => GivenAnInvalidModifyCashflowRequest().Build());

    private static void AValidRequest(IStepRunner runner) =>
        runner.Execute<Cashflow, ModifyCashflow.Request>(item =>
        {
            item.Should().BeValid();
            var request = GivenAModifyCashflowRequest().WithId(item.Value.Id).Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });
}
