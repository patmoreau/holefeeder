using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Application.Features.Accounts.Commands;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;

using static Holefeeder.Tests.Common.Builders.Accounts.ModifyAccountRequestBuilder;

namespace Holefeeder.FunctionalTests.Features.Accounts;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioModifyAccount(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    private const string ModifiedName = nameof(ModifiedName);

    [Fact]
    public Task ModifyingAnAccountWithAnInvalidRequest() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(AnInvalidRequest)
            .When(TheUser.ModifiesAnAccount)
            .Then(ShouldReceiveAValidationError)
            .PlayAsync();

    [Fact]
    public Task WhenAccountNotFound() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(RequestForAnAccountThatDoesNotExists)
            .When(TheUser.ModifiesAnAccount)
            .Then(ShouldNotBeFound)
            .PlayAsync();

    [Fact]
    public Task WhenModifyAccount() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Account.Exists)
            .And(TheUserWantsToModifyTheirAccount)
            .When(TheUser.ModifiesAnAccount)
            .Then(TheNameOfTheAccountShouldBeModified)
            .PlayAsync();

    private static void AnInvalidRequest(IStepRunner runner) =>
        runner.Execute(() => InvalidRequest().Build());

    private static void RequestForAnAccountThatDoesNotExists(IStepRunner runner) =>
        runner.Execute(() => GivenAModifyAccountRequest().Build());

    private static void TheUserWantsToModifyTheirAccount(IStepRunner runner) =>
        runner.Execute<Account, ModifyAccount.Request>(account =>
        {
            account.Should().BeValid();
            return GivenAModifyAccountRequest()
                .WithId(account.Value.Id)
                .WithName(ModifiedName)
                .Build();
        });

    private void TheNameOfTheAccountShouldBeModified(IStepRunner runner) =>
        Account.NameShouldBeModified(runner, ModifiedName);
}
