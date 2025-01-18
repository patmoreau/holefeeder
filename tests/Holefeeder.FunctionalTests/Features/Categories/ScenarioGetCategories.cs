using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Models;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Categories;

public class ScenarioGetCategories(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task WhenCategoriesExists() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Category.Exists)
            .And(Category.Exists)
            .When(TheUser.GetsCategories)
            .Then(TheResultShouldBeAsExpected)
            .PlayAsync();

    [AssertionMethod]
    private static void TheResultShouldBeAsExpected(IStepRunner runner) =>
        runner.Execute<IApiResponse<IEnumerable<CategoryViewModel>>>(response =>
        {
            response.Should().BeValid();
            response.Value.Should().BeSuccessful()
                .And.HaveContent();

            response.Value.Content.Should().HaveCount(2)
                .And.BeInDescendingOrder(x => x.Favorite)
                .And.BeInAscendingOrder(x =>x.Name);
        });
}
