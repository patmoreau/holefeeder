using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Enumerations;

public class ScenarioGetCategoryTypes(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenGettingTheListOfCategoryTypes() =>
        ScenarioRunner.Create(ScenarioOutput)
            .When(TheUser.GetsTheListOfCategoryTypes)
            .Then(TheListShouldContainTheExpectedValues)
            .PlayAsync();

    private static void TheListShouldContainTheExpectedValues(IStepRunner runner) =>
        runner.Execute<IApiResponse<IEnumerable<CategoryType>>>(result =>
        {
            result.Should().BeValid()
                .And.Subject.Value.Should().BeSuccessful()
                .And.HaveEquivalentContent(CategoryType.List);
        });
}
