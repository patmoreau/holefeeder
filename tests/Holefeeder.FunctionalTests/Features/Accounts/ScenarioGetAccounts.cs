using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Features.Accounts.Queries;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Accounts;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetAccounts(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task GettingAccountsWithInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.GetsAccounts)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task GettingAccountsSortedByNameDesc() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.CollectionExists)
            .And(ARequestSortedByNameDesc)
            .When(TheUser.GetsAccounts)
            .Then(ShouldReceiveAccountsInProperOrder)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) => runner.Execute(() => new GetAccounts.Request(-1, 10, [], []));
    private static void ARequestSortedByNameDesc(IStepRunner runner) => runner.Execute(() => new GetAccounts.Request(0, 10, ["-name"], []));

    [AssertionMethod]
    private static void ShouldReceiveAccountsInProperOrder(IStepRunner runner) =>
        runner.Execute<IApiResponse<IEnumerable<AccountViewModel>>>(response =>
        {
            var expected = runner.GetContextData<IEnumerable<Account>>(AccountContexts.ExistingAccounts);
            response.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful();
            var accounts = response.Value;
            accounts.Content.Should().HaveSameCount(expected).And.BeInDescendingOrder(x => x.Name);
        });
}
