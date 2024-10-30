using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Application.Features.Accounts.Commands;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Tests.Common.Builders.Accounts.FavoriteAccountRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Accounts;

public class ScenarioFavoriteAccount(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task SettingAFavoriteAccountWithAnInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.SetsAFavoriteAccount)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task SettingAFavoriteAccountThatDoesNotExists() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(RequestForAnAccountThatDoesNotExists)
            .When(TheUser.SetsAFavoriteAccount)
            .Then(ShouldNotBeFound)
            .PlayAsync();

    [Fact]
    public Task SettingAFavoriteAccount() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(Account.Exists)
            .And(WantsToSetAFavoriteAccount)
            .When(TheUser.SetsAFavoriteAccount)
            .Then(Account.ShouldBeAFavorite)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() => GivenAnInvalidFavoriteAccountRequest().Build());

    private static void RequestForAnAccountThatDoesNotExists(IStepRunner runner) =>
        runner.Execute(() => GivenAFavoriteAccountRequest().Build());


    private static void WantsToSetAFavoriteAccount(IStepRunner runner) =>
        runner.Execute<Account, FavoriteAccount.Request>(account =>
        {
            account.Should().BeValid();
            return GivenAFavoriteAccountRequest().WithId(account.Value.Id).Build();
        });
}
