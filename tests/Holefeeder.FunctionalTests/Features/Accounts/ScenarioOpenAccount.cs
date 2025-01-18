using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Application.Features.Accounts.Commands.OpenAccount;
using static Holefeeder.Tests.Common.Builders.Accounts.OpenAccountRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioOpenAccount(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    private const string NewAccountName = nameof(NewAccountName);

    [Fact]
    public Task WhenInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.OpensAnAccount)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task WhenAccountNameAlreadyExistsRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(ARequestWithExistingAccountName)
            .When(TheUser.OpensAnAccount)
            .Then(ShouldExpectBadRequest)
            .PlayAsync();

    [Fact]
    public Task WhenOpenAccount() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(ARequestForANewAccount)
            .When(TheUser.OpensAnAccount)
            .Then(TheAccountShouldBeCreated)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() => GivenAnInvalidOpenAccountRequest().Build());

    private static void ARequestWithExistingAccountName(IStepRunner runner) =>
        runner.Execute<Account, Request>(account =>
        {
            account.Should().BeValid();
            return GivenAnOpenAccountRequest()
                .WithName(account.Value.Name)
                .Build();
        });

    private static void ARequestForANewAccount(IStepRunner runner) =>
        runner.Execute(() => GivenAnOpenAccountRequest().WithName(NewAccountName).Build());

    private void TheAccountShouldBeCreated(IStepRunner runner) =>
        Account.ShouldBeCreated(runner, NewAccountName);
}
