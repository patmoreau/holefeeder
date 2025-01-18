using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Features.Transactions.Queries;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Transactions;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Transactions;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetTransactions(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task GettingTransactionsWithInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.GetsTransactions)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task GettingTransactionsSortedByDescriptionDesc() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Category.Exists)
            .And(Transaction.CollectionExists)
            .And(ARequestSortedByDescriptionDesc)
            .When(TheUser.GetsTransactions)
            .Then(ShouldReceiveItemsInProperOrder)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) => runner.Execute(() => new GetTransactions.Request(-1, 10, [], []));

    private static void ARequestSortedByDescriptionDesc(IStepRunner runner) => runner.Execute(() => new GetTransactions.Request(0, 10, ["-description"], []));

    [AssertionMethod]
    private static void ShouldReceiveItemsInProperOrder(IStepRunner runner) =>
        runner.Execute<IApiResponse<IEnumerable<TransactionInfoViewModel>>>(response =>
        {
            var expected = runner.GetContextData<IEnumerable<Transaction>>(TransactionContexts.ExistingTransactions);
            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful();
            var accounts = response.Value;
            accounts.Content.Should().HaveSameCount(expected).And.BeInDescendingOrder(x => x.Description);
        });
}
