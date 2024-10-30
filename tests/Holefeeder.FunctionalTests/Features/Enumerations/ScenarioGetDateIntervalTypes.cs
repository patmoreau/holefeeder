using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Domain.Enumerations;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Enumerations;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetDateInternalTypes(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenGettingTheListOfDateInternalTypes() =>
        ScenarioRunner.Create(ScenarioOutput)
            .When(TheUser.GetsTheListOfDateIntervalTypes)
            .Then(TheListShouldContainTheExpectedValues)
            .PlayAsync();

    private static void TheListShouldContainTheExpectedValues(IStepRunner runner) =>
        runner.Execute<IApiResponse<IEnumerable<DateIntervalType>>>(result =>
        {
            result.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveEquivalentContent(DateIntervalType.List);
        });
}
