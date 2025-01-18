using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Application.Features.Transactions.Commands.DeleteTransaction;
using static Holefeeder.Tests.Common.Builders.Transactions.DeleteTransactionRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioDeleteTransaction(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.DeletesATransaction)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task WhenDeletingATransaction() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Transaction.Exists)
            .And(AValidRequest)
            .When(TheUser.DeletesATransaction)
            .Then(Transaction.ShouldNotBeFound)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() => GivenAnInvalidDeleteTransactionRequest().Build());

    private static void AValidRequest(IStepRunner runner) =>
        runner.Execute<Transaction, Request>(transaction =>
        {
            transaction.Should().BeValid();
            var request = GivenADeleteTransactionRequest().WithId(transaction.Value.Id).Build();
            runner.SetContextData(RequestContext.CurrentRequest, request);
            return request;
        });
}
