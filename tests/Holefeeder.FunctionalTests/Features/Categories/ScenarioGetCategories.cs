using DrifterApps.Seeds.FluentScenario;
using DrifterApps.Seeds.FluentScenario.Attributes;

using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Categories;

public class ScenarioGetCategories(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public Task GettingCategories() =>
        ScenarioRunner.Create(ScenarioOutput)
            .Given(Category.CollectionExists)
            .When(TheUser.GetsCategories)
            .Then(ShouldReceiveAllCategoriesOrderedByDescendingFavoriteAndAscendingName)
            .PlayAsync();

    [AssertionMethod]
    private static void ShouldReceiveAllCategoriesOrderedByDescendingFavoriteAndAscendingName(IStepRunner runner) =>
        runner.Execute<IApiResponse<IEnumerable<CategoryViewModel>>>(response =>
        {
            var categories = runner.GetContextData<IEnumerable<Category>>(CategoryContext.ExistingCategories);

            response.Should().BeValid();
            response.Value.Should().BeSuccessful()
                .And.HaveContent();

            response.Value.Content.Should().HaveSameCount(categories)
                .And.BeInDescendingOrder(x => x.Favorite)
                .And.BeInAscendingOrder(x =>x.Name);
        });
}
