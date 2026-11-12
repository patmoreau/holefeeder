using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Categories.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using static Holefeeder.ServiceTests.Categories.TestKit.CategoryForTest;

namespace Holefeeder.ServiceTests.Categories.Api.DeleteCategory;

[Collection(ComponentTestMarker.Name)]
public class DeleteCategoryComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Category _dining = null!;
    private Category _groceries = null!;
    private Category _transferOut = null!;
    private Guid _bistro;

    [Fact]
    public Task WhenTheCategoryIsDeleted() =>
        ScenarioRunner.Create("Deleting a category", _scenarioOutput)
            .Given("a registered user who ate out once", ARegisteredUserWhoAteOutOnce)
            .When("the user deletes dining", () => DeletingTheCategory(_user, _dining.Id.Value))
            .Then<HttpResponseMessage>("dining leaves the list but the meal keeps it", DiningLeavesTheListButTheMealKeepsIt)
            .PlayAsync();

    [Fact]
    public Task WhenTheCategoryIsASystemOne() =>
        ScenarioRunner.Create("Deleting a system category", _scenarioOutput)
            .Given("a registered user who ate out once", ARegisteredUserWhoAteOutOnce)
            .When("the user deletes transfer out", () => DeletingTheCategory(_user, _transferOut.Id.Value))
            .Then<HttpResponseMessage>("the deletion is refused", response => AProblemIsReturned(response, HttpStatusCode.BadRequest, CategoryErrors.System.Description))
            .PlayAsync();

    [Fact]
    public Task WhenTheCategoryIsSomeoneElses() =>
        ScenarioRunner.Create("Deleting someone else's category", _scenarioOutput)
            .Given("a registered user who ate out once", ARegisteredUserWhoAteOutOnce)
            .When("another user deletes dining", DeletingTheCategoryAsAnotherUser)
            .Then<HttpResponseMessage>("the category is not found", response => AProblemIsReturned(response, HttpStatusCode.NotFound, CategoryErrors.NotFound.Description))
            .PlayAsync();

    [Fact]
    public Task WhenTheIdIsEmpty() =>
        ScenarioRunner.Create("Deleting a category with an empty id", _scenarioOutput)
            .Given("a registered user who ate out once", ARegisteredUserWhoAteOutOnce)
            .When("the user deletes a category with an empty id", () => DeletingTheCategory(_user, Guid.Empty))
            .Then<HttpResponseMessage>("a validation error is returned on the id", AValidationErrorIsReturnedOnTheId)
            .PlayAsync();

    private async Task ARegisteredUserWhoAteOutOnce()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        _dining = await ACategory("Dining", color: "#FF9500").SavedInDbAsync(_user, server.Database);
        _groceries = await ACategory("Groceries").SavedInDbAsync(_user, server.Database);
        _transferOut = await ACategory("Transfer Out", color: "#607D8B", system: true).SavedInDbAsync(_user, server.Database);
        var budget = new BudgetForTest(_user, server.Database);
        var chequing = await budget.AnAccountAsync("Chequing", AccountType.Checking);
        _bistro = await budget.ATransactionAsync(_dining.Id.Value, new DateOnly(2026, 8, 28), 42m, chequing, "Bistro");
    }

    private async Task<HttpResponseMessage> DeletingTheCategory(User user, Guid id)
    {
        using var client = server.CreateClientFor(user.IdentityObjectId);
        return await client.DeleteAsync(new Uri($"/api/v2/categories/{id}", UriKind.Relative));
    }

    private async Task<HttpResponseMessage> DeletingTheCategoryAsAnotherUser()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        return await DeletingTheCategory(otherUser, _dining.Id.Value);
    }

    private async Task DiningLeavesTheListButTheMealKeepsIt(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        var categories = await client.GetFromJsonAsync<List<CategoryJson>>(new Uri("/api/v2/categories/manage", UriKind.Relative));
        categories!.Select(category => category.Id).Should().Equal(_groceries.Id.Value, _transferOut.Id.Value);
        var meal = await client.GetFromJsonAsync<TransactionJson>(new Uri($"/api/v2/transactions/{_bistro}", UriKind.Relative));
        meal!.Category.Should().Be(new TransactionCategoryJson(_dining.Id.Value, "Dining"));
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

    private sealed record CategoryJson(Guid Id);

    private sealed record TransactionJson(TransactionCategoryJson Category);

    private sealed record TransactionCategoryJson(Guid Id, string Name);
}
