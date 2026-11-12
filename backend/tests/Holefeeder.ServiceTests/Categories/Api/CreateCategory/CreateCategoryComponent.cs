using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using static Holefeeder.ServiceTests.Categories.TestKit.CategoryForTest;

namespace Holefeeder.ServiceTests.Categories.Api.CreateCategory;

[Collection(ComponentTestMarker.Name)]
public class CreateCategoryComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;

    [Fact]
    public Task WhenTheCategoryIsNew() =>
        ScenarioRunner.Create("Creating a category", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user creates a dining category", () => CreatingTheCategory(new { Name = "Dining", Type = "expense", Color = "#FF9500", BudgetAmount = 200.5, Favorite = true }))
            .Then<HttpResponseMessage>("the category is listed", TheCategoryIsListed)
            .PlayAsync();

    [Fact]
    public Task WhenTheNameIsAlreadyUsed() =>
        ScenarioRunner.Create("Creating a category with a name already used", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user deleted a dining category", TheUserDeletedADiningCategory)
            .When("the user creates a dining category", () => CreatingTheCategory(new { Name = "Dining", Type = "expense", Color = "#FF9500", BudgetAmount = "0", Favorite = false }))
            .Then<HttpResponseMessage>("the name is reported as already used", TheNameIsReportedAsAlreadyUsed)
            .PlayAsync();

    [Fact]
    public Task WhenTheRequestIsInvalid() =>
        ScenarioRunner.Create("Creating a category with invalid values", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user creates a category without name, type, color or budget", () => CreatingTheCategory(new { Name = "" }))
            .Then<HttpResponseMessage>("a validation error is returned on each value", AValidationErrorIsReturnedOnEachValue)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Creating a category as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .When("the user creates a dining category", () => CreatingTheCategory(new { Name = "Dining", Type = "expense", Color = "#FF9500", BudgetAmount = 0, Favorite = false }))
            .Then<HttpResponseMessage>("the user is reported as not registered", TheUserIsReportedAsNotRegistered)
            .PlayAsync();

    private async Task ARegisteredUser() => _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private Task TheUserDeletedADiningCategory() => ACategory("Dining").SavedInDbAsync(_user, server.Database, inactive: true);

    private async Task<HttpResponseMessage> CreatingTheCategory(object body)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(new Uri("/api/v2/categories", UriKind.Relative), body);
    }

    private async Task TheCategoryIsListed(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Value.Content.ReadFromJsonAsync<CreatedJson>();
        created.Should().NotBeNull();

        using var client = server.CreateClientFor(_user.IdentityObjectId);
        var categories = await client.GetFromJsonAsync<List<CategoryJson>>(new Uri("/api/v2/categories/manage", UriKind.Relative));
        categories.Should().Equal(new CategoryJson(created.Id, "Dining", "expense", "#FF9500", 200.5m, true, false));
    }

    private static async Task TheNameIsReportedAsAlreadyUsed(Ensure<HttpResponseMessage> response) =>
        (await AProblemAsync(response)).Detail.Should().Be(CategoryErrors.NameTaken.Description);

    private static async Task TheUserIsReportedAsNotRegistered(Ensure<HttpResponseMessage> response) =>
        (await AProblemAsync(response)).Detail.Should().Be(UserErrors.NotRegistered.Description);

    private static async Task AValidationErrorIsReturnedOnEachValue(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Errors.Should().ContainKeys("category-name-invalid", "category-type-invalid", "category-color-invalid", "amount-invalid");
    }

    private static async Task<ProblemDetails> AProblemAsync(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        return problem;
    }

    private sealed record CreatedJson(Guid Id);

    private sealed record CategoryJson(Guid Id, string Name, string Type, string Color, decimal BudgetAmount, bool Favorite, bool System);
}
