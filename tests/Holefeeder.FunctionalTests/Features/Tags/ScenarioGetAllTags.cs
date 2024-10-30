using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Application.Features.Tags.Queries;
using Holefeeder.Domain.Features.Accounts;
using Holefeeder.Domain.Features.Categories;
using Holefeeder.Domain.Features.Users;
using Holefeeder.FunctionalTests.Drivers;
using Holefeeder.Tests.Common.Builders.Accounts;
using Holefeeder.Tests.Common.Builders.Categories;
using Holefeeder.Tests.Common.Builders.Transactions;

using Refit;

namespace Holefeeder.FunctionalTests.Features.Tags;

[ComponentTest]
[Collection("Api collection")]
public class ScenarioGetAllTags(ApiApplicationDriver applicationDriver, ITestOutputHelper testOutputHelper) : HolefeederScenario(applicationDriver, testOutputHelper)
{
    private readonly UserId _userId = TestUsers[AuthorizedUser].UserId;

    private readonly Dictionary<string, Category> _categories = new();
    private readonly Dictionary<string, Account> _accounts = new();

    [Fact]
    public Task WhenGettingTagsWithCount() =>
        ScenarioRunner.Create("A user gets his tags", ScenarioOutput)
            .Given("a 'purchase' transaction was made on the 'credit card' with tag 'groceries'", () => CreateTransaction("purchase", "credit card", "groceries"))
            .And("a 'food and drink' transaction was made on the 'credit card' account with tag 'groceries'", () => CreateTransaction("food and drink", "credit card", "groceries"))
            .And("a 'food and drink' transaction was made on the 'credit card' account with tag 'restaurant'", () => CreateTransaction("food and drink", "credit card", "restaurant"))
            .And("a 'food and drink' transaction was made on the 'credit card' account with tags 'restaurant' and 'alcohol'", () => CreateTransaction("food and drink", "credit card", "restaurant", "alcohol"))
            .And("a second 'food and drink' transaction was made on the 'checking' account with tag 'groceries'", () => CreateTransaction("food and drink", "checking", "groceries"))
            .When(TheUser.GetsTheirTags)
            .Then<IApiResponse<IEnumerable<TagDto>>>("the results by CategoryId should match the expected", ValidateResponse)
            .PlayAsync();

    private void ValidateResponse(Ensure<IApiResponse<IEnumerable<TagDto>>> response)
    {
        var expectedForGroceries = new TagDto("groceries", 3);
        var expectedForRestaurant = new TagDto("restaurant", 2);
        var expectedForAlcohol = new TagDto("alcohol", 1);

        response.Should().BeValid()
            .And.Subject.Value.Should().BeSuccessful()
            .And.HaveContent();
        var results = response.Value.Content;
        results.Should()
            .NotBeNull()
            .And.HaveCount(3)
            .And.ContainInOrder(expectedForGroceries, expectedForRestaurant, expectedForAlcohol);
    }

    private async Task CreateTransaction(string categoryName, string accountName, params string[] tags)
    {
        if (!_categories.TryGetValue(categoryName, out var category))
        {
            category = await CategoryBuilder.GivenACategory().OfType(CategoryType.Expense).WithName(categoryName).ForUser(_userId).SavedInDbAsync(DatabaseDriver);
            _categories.Add(categoryName, category);
        }

        if (!_accounts.TryGetValue(accountName, out var account))
        {
            account = await AccountBuilder.GivenAnActiveAccount().WithName(accountName).ForUser(_userId).SavedInDbAsync(DatabaseDriver);
            _accounts.Add(accountName, account);
        }

        var builder = TransactionBuilder
            .GivenATransaction()
            .ForAccount(account)
            .ForCategory(category);

        if (tags.Length > 0)
            builder.WithTags(tags);
        else
            builder.WithNoTags();

        await builder.SavedInDbAsync(DatabaseDriver);
    }
}
