using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Application.Features.Accounts.Commands;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Tests.Common.Builders.Accounts.CloseAccountRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class CloseAccountScenario(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task ClosingAnAccountWithAnInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.ClosesTheAccount)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task ClosingAnAccountThatDoesNotExists() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(RequestForAnAccountThatDoesNotExists)
            .When(TheUser.ClosesTheAccount)
            .Then(ShouldNotBeFound)
            .PlayAsync();

    [Fact]
    public Task ClosingAnAccount() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(WantsToCloseThatExistingAccount)
            .When(TheUser.ClosesTheAccount)
            .Then(Account.ShouldBeClosed)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() => GivenAnInvalidCloseAccountRequest().Build());

    private static void RequestForAnAccountThatDoesNotExists(IStepRunner runner) =>
        runner.Execute(() => GivenACloseAccountRequest().Build());

    private static void WantsToCloseThatExistingAccount(IStepRunner runner) =>
        runner.Execute<Account, CloseAccount.Request>(account =>
        {
            account.Should().BeValid();
            return GivenACloseAccountRequest().WithId(account.Value.Id).Build();
        });
}
