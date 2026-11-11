using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.ServiceTests.Accounts.Api.GetAccountDetail;

[Collection(ComponentTestMarker.Name)]
public class GetAccountDetailComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Now);
    private static readonly DateOnly CurrentMonth = new(Today.Year, Today.Month, 1);
    private static readonly DateOnly OpenDate = new(2025, 3, 1);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _amex;

    [Fact]
    public Task WhenTheAccountExists() =>
        ScenarioRunner.Create("Getting the detail of an account", _scenarioOutput)
            .Given("a registered user with monthly settings", ARegisteredUserWithMonthlySettings)
            .And("the user has a credit card with an upcoming payment", TheUserHasACreditCardWithAnUpcomingPayment)
            .When("the user gets the detail of the credit card", () => GettingTheAccountDetail(_amex))
            .Then<HttpResponseMessage>("the account shows its balance, upcoming and projection", TheAccountShowsItsBalanceUpcomingAndProjection)
            .PlayAsync();

    [Fact]
    public Task WhenTheAccountBelongsToAnotherUser() =>
        ScenarioRunner.Create("Getting the detail of another user's account", _scenarioOutput)
            .Given("a registered user with monthly settings", ARegisteredUserWithMonthlySettings)
            .And("another user has a credit card", AnotherUserHasACreditCard)
            .When("the user gets the detail of the other user's credit card", () => GettingTheAccountDetail(_amex))
            .Then<HttpResponseMessage>("the account is not found", TheAccountIsNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenTheIdIsEmpty() =>
        ScenarioRunner.Create("Getting the detail of an account with an empty id", _scenarioOutput)
            .Given("a registered user with monthly settings", ARegisteredUserWithMonthlySettings)
            .When("the user gets the detail with an empty id", () => GettingTheAccountDetail(Guid.Empty))
            .Then<HttpResponseMessage>("a validation error is returned on the id", AValidationErrorIsReturnedOnTheId)
            .PlayAsync();

    private async Task ARegisteredUserWithMonthlySettings()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        await new BudgetForTest(_user, server.Database).SettingsAsync(
            $$"""{"effectiveDate":"{{Today.Year - 1}}-01-01","intervalType":"monthly","frequency":1}""");
    }

    private async Task TheUserHasACreditCardWithAnUpcomingPayment()
    {
        var budget = new BudgetForTest(_user, server.Database);
        _amex = await budget.AnAccountAsync("Amex", AccountType.CreditCard, openBalance: 0m, openDate: OpenDate, favorite: true,
            description: "Travel card");
        var food = await budget.ACategoryAsync(CategoryType.Expense);
        await budget.ATransactionAsync(food, CurrentMonth, 123.45m, _amex);
        await budget.ACashflowAsync(food, Today, IntervalType.OneTime, 1, 66.67m, accountId: _amex);
    }

    private async Task AnotherUserHasACreditCard()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        _amex = await new BudgetForTest(otherUser, server.Database).AnAccountAsync("Amex", AccountType.CreditCard);
    }

    private async Task<HttpResponseMessage> GettingTheAccountDetail(Guid id)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri($"/api/v2/accounts/{id}/detail", UriKind.Relative));
    }

    private async Task TheAccountShowsItsBalanceUpcomingAndProjection(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var account = await response.Value.Content.ReadFromJsonAsync<AccountDetailJson>();
        account.Should().Be(new AccountDetailJson(
            _amex, "Amex", "CreditCard", "Travel card", 0m, OpenDate, Favorite: true, Inactive: false,
            "amountDue", -123.45m, -66.67m, -190.12m, CurrentMonth));
    }

    private static async Task TheAccountIsNotFound(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(AccountErrors.NotFound.Description);
    }

    private static async Task AValidationErrorIsReturnedOnTheId(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Errors.Should().ContainKey(IdErrors.CodeInvalid);
    }

    private sealed record AccountDetailJson(
        Guid Id,
        string Name,
        string Type,
        string Description,
        decimal OpenBalance,
        DateOnly OpenDate,
        bool Favorite,
        bool Inactive,
        string Caption,
        decimal Balance,
        decimal Upcoming,
        decimal ProjectedBalance,
        DateOnly Updated);
}
