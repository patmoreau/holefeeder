using System.Net;
using Holefeeder.Application.Models;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.FunctionalTests.Extensions;
using Holefeeder.FunctionalTests.Infrastructure;
using static Holefeeder.FunctionalTests.Infrastructure.MockAuthenticationHandler;
using static Holefeeder.Tests.Common.Builders.Categories.CategoryBuilder;

namespace Holefeeder.FunctionalTests.Features.Categories;

public class ScenarioGetCategories : BaseScenario
{
    public ScenarioGetCategories(ApiApplicationDriver apiApplicationDriver, BudgetingDatabaseInitializer budgetingDatabaseInitializer, ITestOutputHelper testOutputHelper)
        : base(apiApplicationDriver, budgetingDatabaseInitializer, testOutputHelper)
    {
    }

    [Fact]
    public async Task WhenCategoriesExists()
    {
        const string firstName = nameof(firstName);
        const string secondName = nameof(secondName);

        Category firstCategory = await GivenACategory()
            .WithName(firstName)
            .ForUser(AuthorizedUserId)
            .IsNotFavorite()
            .SavedInDb(DatabaseDriver);

        Category secondCategory = await GivenACategory()
            .WithName(secondName)
            .ForUser(AuthorizedUserId)
            .IsFavorite()
            .SavedInDb(DatabaseDriver);

        GivenUserIsAuthorized();

        await WhenUserGetCategories();

        ThenShouldExpectStatusCode(HttpStatusCode.OK);
        CategoryViewModel[]? result = HttpClientDriver.DeserializeContent<CategoryViewModel[]>();
        ThenAssertAll(() =>
        {
            result.Should().NotBeNull().And.HaveCount(2).And.BeInDescendingOrder(x => x.Favorite);
            result![0].Should().BeEquivalentTo(secondCategory,
                options => options.ExcludingMissingMembers());
            result[1].Should().BeEquivalentTo(firstCategory,
                options => options.ExcludingMissingMembers());
        });
    }

    private async Task WhenUserGetCategories() => await HttpClientDriver.SendGetRequest(ApiResource.GetCategories);
}
