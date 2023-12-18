using System.Net;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.FunctionalTests.StepDefinitions.UserStepDefinition;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;

namespace Holefeeder.FunctionalTests.Features.Categories;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetCategories : HolefeederScenario
{
    public ScenarioGetCategories(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper)
        : base(applicationDriver, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenCategoriesExists()
    {
        const string firstName = nameof(firstName);
        const string secondName = nameof(secondName);

        Category firstCategory = await GivenACategory()
            .WithName(firstName)
            .ForUser(HolefeederUserId)
            .IsNotFavorite()
            .SavedInDbAsync(DatabaseDriver);

        Category secondCategory = await GivenACategory()
            .WithName(secondName)
            .ForUser(HolefeederUserId)
            .IsFavorite()
            .SavedInDbAsync(DatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetCategories();

        ShouldExpectStatusCode(HttpStatusCode.OK);
        CategoryViewModel[]? result = HttpClientDriver.DeserializeContent<CategoryViewModel[]>();
        AssertAll(() =>
        {
            result.Should().NotBeNull().And.HaveCount(2).And.BeInDescendingOrder(x => x.Favorite);
            result![0].Should().BeEquivalentTo(secondCategory,
                options => options.ExcludingMissingMembers());
            result[1].Should().BeEquivalentTo(firstCategory,
                options => options.ExcludingMissingMembers());
        });
    }

    private async Task WhenUserGetCategories() => await HttpClientDriver.SendGetRequestAsync(ApiResources.GetCategories);
}
