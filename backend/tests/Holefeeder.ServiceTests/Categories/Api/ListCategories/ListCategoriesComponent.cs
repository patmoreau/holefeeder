using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

using static Holefeeder.ServiceTests.Categories.TestKit.CategoryForTest;

namespace Holefeeder.ServiceTests.Categories.Api.ListCategories;

[Collection(ComponentTestMarker.Name)]
public class ListCategoriesComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _groceries;
    private Guid _salary;
    private Guid _transferIn;

    [Fact]
    public Task WhenTheUserHasCategories() =>
        ScenarioRunner.Create("Listing the categories to manage", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has categories", TheUserHasCategories)
            .When("the user lists the categories", ListingTheCategories)
            .Then<HttpResponseMessage>("the active categories are listed in order", TheActiveCategoriesAreListed)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Listing the categories as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .When("the user lists the categories", ListingTheCategories)
            .Then<HttpResponseMessage>("nothing is listed", NothingIsListed)
            .PlayAsync();

    private async Task ARegisteredUser() => _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserHasCategories()
    {
        _salary = (await ACategory("Salary", CategoryType.Gain, "#30B0C7").SavedInDbAsync(_user, server.Database)).Id.Value;
        _groceries = (await ACategory("Groceries", budget: 600m, favorite: true).SavedInDbAsync(_user, server.Database)).Id.Value;
        _transferIn = (await ACategory("Transfer In", CategoryType.Gain, "#607D8B", system: true).SavedInDbAsync(_user, server.Database)).Id.Value;
        await ACategory("Old").SavedInDbAsync(_user, server.Database, inactive: true);
    }

    private async Task<HttpResponseMessage> ListingTheCategories()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri("/api/v2/categories/manage", UriKind.Relative));
    }

    private async Task TheActiveCategoriesAreListed(Ensure<HttpResponseMessage> response)
    {
        var categories = await TheCategoriesAsync(response);
        categories.Should().Equal(
            new CategoryJson(_groceries, "Groceries", "expense", "#34C759", 600m, true, false),
            new CategoryJson(_salary, "Salary", "gain", "#30B0C7", 0m, false, false),
            new CategoryJson(_transferIn, "Transfer In", "gain", "#607D8B", 0m, false, true));
    }

    private static async Task NothingIsListed(Ensure<HttpResponseMessage> response) =>
        (await TheCategoriesAsync(response)).Should().BeEmpty();

    private static async Task<List<CategoryJson>> TheCategoriesAsync(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        return (await response.Value.Content.ReadFromJsonAsync<List<CategoryJson>>())!;
    }

    private sealed record CategoryJson(Guid Id, string Name, string Type, string Color, decimal BudgetAmount, bool Favorite, bool System);
}
