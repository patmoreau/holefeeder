using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Domain.Features.Accounts;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Enumerations;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetAccountTypes(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenGettingTheListOfAccountTypes() =>
    ScenarioRunner.Create(ScenarioOutput)
        .When(TheUser.GetsTheListOfAccountTypes)
        .Then(TheListShouldContainTheExpectedValues)
        .PlayAsync();

    private static void TheListShouldContainTheExpectedValues(IStepRunner runner) =>
        runner.Execute<IApiResponse<IEnumerable<AccountType>>>(result =>
        {
            result.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveEquivalentContent(AccountType.List);
        });
}
