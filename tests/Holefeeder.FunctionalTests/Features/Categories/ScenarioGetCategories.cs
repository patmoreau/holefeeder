using System.Net;

using Holefeeder.Application.Models;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;

using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;

namespace Holefeeder.FunctionalTests.Features.Categories;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetCategories(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
    : HolefeederScenario(applicationDriver, testOutputHelper)
{
    [Fact]
    public async Task WhenCategoriesExists()
    {
        const string firstName = nameof(firstName);
        const string secondName = nameof(secondName);

        var firstCategory = await GivenACategory()
            .WithName(firstName)
            .ForUser(TestUsers[AuthorizedUser].UserId)
            .IsNotFavorite()
            .SavedInDbAsync(DatabaseDriver);

        var secondCategory = await GivenACategory()
            .WithName(secondName)
            .ForUser(TestUsers[AuthorizedUser].UserId)
            .IsFavorite()
            .SavedInDbAsync(DatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetCategories();

        ShouldExpectStatusCode(HttpStatusCode.OK);
        var result = HttpClientDriver.DeserializeContent<CategoryViewModel[]>();
        AssertAll(() =>
        {
            result.Should().NotBeNull().And.HaveCount(2).And.BeInDescendingOrder(x => x.Favorite);
            result![0].Should().BeEquivalentTo(secondCategory,
                options => options.ExcludingMissingMembers());
            result[1].Should().BeEquivalentTo(firstCategory,
                options => options.ExcludingMissingMembers());
        });
    }

    private async Task WhenUserGetCategories() => await HttpClientDriver.SendRequestAsync(ApiResources.GetCategories);
}
