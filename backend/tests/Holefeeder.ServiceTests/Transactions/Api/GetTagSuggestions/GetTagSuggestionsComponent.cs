using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Dashboard.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

namespace Holefeeder.ServiceTests.Transactions.Api.GetTagSuggestions;

[Collection(ComponentTestMarker.Name)]
public class GetTagSuggestionsComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Day = new(2026, 11, 1);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _groceries;
    private string? _categoryId;

    [Fact]
    public Task WhenACategoryIsChosen() =>
        ScenarioRunner.Create("Getting tag suggestions for a category", _scenarioOutput)
            .Given("a registered user who tagged transactions", ARegisteredUserWhoTaggedTransactions)
            .And("the groceries category is chosen", TheGroceriesCategoryIsChosen)
            .When("the user gets tag suggestions", GettingTagSuggestions)
            .Then<HttpResponseMessage>("tags used with groceries come first", TagsUsedWithGroceriesComeFirst)
            .PlayAsync();

    [Fact]
    public Task WhenTheCategoryIsNotAnId() =>
        ScenarioRunner.Create("Getting tag suggestions for a category that is not an id", _scenarioOutput)
            .Given("a registered user who tagged transactions", ARegisteredUserWhoTaggedTransactions)
            .And("a category that is not an id is chosen", ACategoryThatIsNotAnIdIsChosen)
            .When("the user gets tag suggestions", GettingTagSuggestions)
            .Then<HttpResponseMessage>("the request is rejected", TheRequestIsRejected)
            .PlayAsync();

    private async Task ARegisteredUserWhoTaggedTransactions()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        var budget = new BudgetForTest(_user, server.Database);
        _groceries = await budget.ACategoryAsync(CategoryType.Expense, name: "Groceries");
        var dining = await budget.ACategoryAsync(CategoryType.Expense, name: "Dining");
        await budget.ATransactionAsync(dining, Day, 10m, tags: "family");
        await budget.ATransactionAsync(dining, Day, 10m, tags: "family");
        await budget.ATransactionAsync(_groceries, Day, 10m, tags: "costco");
    }

    private void TheGroceriesCategoryIsChosen() => _categoryId = _groceries.ToString();

    private void ACategoryThatIsNotAnIdIsChosen() => _categoryId = "groceries";

    private async Task<HttpResponseMessage> GettingTagSuggestions()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri($"/api/v2/transactions/tag-suggestions?categoryId={_categoryId}", UriKind.Relative));
    }

    private static async Task TagsUsedWithGroceriesComeFirst(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var tags = await response.Value.Content.ReadFromJsonAsync<List<string>>();
        tags.Should().Equal("costco", "family");
    }

    private static void TheRequestIsRejected(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
