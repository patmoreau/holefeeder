using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using static Holefeeder.ServiceTests.Categories.TestKit.CategoryForTest;

namespace Holefeeder.ServiceTests.Categories.Api.ModifyCategory;

[Collection(ComponentTestMarker.Name)]
public class ModifyCategoryComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Category _dining = null!;
    private Category _transferIn = null!;

    [Fact]
    public Task WhenTheCategoryIsModified() =>
        ScenarioRunner.Create("Modifying a category", _scenarioOutput)
            .Given("a registered user with categories", ARegisteredUserWithCategories)
            .When("the user renames dining to restaurants", () => ModifyingTheCategory(_user, ABody(_dining.Id.Value, "Restaurants")))
            .Then<HttpResponseMessage>("the category is listed with its new values", TheCategoryIsListedWithItsNewValues)
            .PlayAsync();

    [Fact]
    public Task WhenTheCategoryIsASystemOne() =>
        ScenarioRunner.Create("Modifying a system category", _scenarioOutput)
            .Given("a registered user with categories", ARegisteredUserWithCategories)
            .When("the user renames transfer in", () => ModifyingTheCategory(_user, ABody(_transferIn.Id.Value, "Money In")))
            .Then<HttpResponseMessage>("the change is refused", response => AProblemIsReturned(response, HttpStatusCode.BadRequest, CategoryErrors.System.Description))
            .PlayAsync();

    [Fact]
    public Task WhenTheCategoryIsSomeoneElses() =>
        ScenarioRunner.Create("Modifying someone else's category", _scenarioOutput)
            .Given("a registered user with categories", ARegisteredUserWithCategories)
            .When("another user renames dining", ModifyingTheCategoryAsAnotherUser)
            .Then<HttpResponseMessage>("the category is not found", response => AProblemIsReturned(response, HttpStatusCode.NotFound, CategoryErrors.NotFound.Description))
            .PlayAsync();

    [Fact]
    public Task WhenTheIdIsEmpty() =>
        ScenarioRunner.Create("Modifying a category with an empty id", _scenarioOutput)
            .Given("a registered user with categories", ARegisteredUserWithCategories)
            .When("the user modifies a category with an empty id", () => ModifyingTheCategory(_user, ABody(Guid.Empty, "Dining")))
            .Then<HttpResponseMessage>("a validation error is returned on the id", AValidationErrorIsReturnedOnTheId)
            .PlayAsync();

    private async Task ARegisteredUserWithCategories()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        _dining = await ACategory("Dining").SavedInDbAsync(_user, server.Database);
        _transferIn = await ACategory("Transfer In", CategoryType.Gain, "#607D8B", system: true).SavedInDbAsync(_user, server.Database);
    }

    private static object ABody(Guid id, string name) =>
        new { Id = id, Name = name, Type = "expense", Color = "#AF52DE", BudgetAmount = "125.50", Favorite = true };

    private async Task<HttpResponseMessage> ModifyingTheCategory(User user, object body)
    {
        using var client = server.CreateClientFor(user.IdentityObjectId);
        return await client.PostAsJsonAsync(new Uri("/api/v2/categories/modify", UriKind.Relative), body);
    }

    private async Task<HttpResponseMessage> ModifyingTheCategoryAsAnotherUser()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        return await ModifyingTheCategory(otherUser, ABody(_dining.Id.Value, "Mine"));
    }

    private async Task TheCategoryIsListedWithItsNewValues(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        var categories = await client.GetFromJsonAsync<List<CategoryJson>>(new Uri("/api/v2/categories/manage", UriKind.Relative));
        categories.Should().Equal(
            new CategoryJson(_dining.Id.Value, "Restaurants", "expense", "#AF52DE", 125.50m, true, false),
            new CategoryJson(_transferIn.Id.Value, "Transfer In", "gain", "#607D8B", 0m, false, true));
    }

    private static async Task AProblemIsReturned(Ensure<HttpResponseMessage> response, HttpStatusCode status, string detail)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(status);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(detail);
    }

    private static async Task AValidationErrorIsReturnedOnTheId(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Errors.Should().ContainKey(IdErrors.CodeInvalid);
    }

    private sealed record CategoryJson(Guid Id, string Name, string Type, string Color, decimal BudgetAmount, bool Favorite, bool System);
}
