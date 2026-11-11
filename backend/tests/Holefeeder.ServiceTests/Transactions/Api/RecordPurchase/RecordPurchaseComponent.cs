using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Dashboard.Core.GetDashboardAccounts;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Dashboard.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.Transactions.TestKit;

namespace Holefeeder.ServiceTests.Transactions.Api.RecordPurchase;

[Collection(ComponentTestMarker.Name)]
public class RecordPurchaseComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _chequing;
    private Guid _groceries;
    private object _body = null!;

    [Fact]
    public Task WhenThePurchaseRepeats() =>
        ScenarioRunner.Create("Recording a repeating purchase", _scenarioOutput)
            .Given("a registered user with an account and a category", ARegisteredUserWithAnAccountAndACategory)
            .And("a monthly purchase", AMonthlyPurchase)
            .When("the user records the purchase", RecordingThePurchase)
            .Then<HttpResponseMessage>("the purchase and its cashflow are recorded", ThePurchaseAndItsCashflowAreRecorded)
            .PlayAsync();

    [Fact]
    public Task WhenTheAccountIsSomeoneElses() =>
        ScenarioRunner.Create("Recording a purchase on someone else's account", _scenarioOutput)
            .Given("a registered user with an account and a category", ARegisteredUserWithAnAccountAndACategory)
            .And("a purchase on another user's account", APurchaseOnAnotherUsersAccount)
            .When("the user records the purchase", RecordingThePurchase)
            .Then<HttpResponseMessage>("the account is not found", TheAccountIsNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenThePurchaseIsInvalid() =>
        ScenarioRunner.Create("Recording an invalid purchase", _scenarioOutput)
            .Given("a registered user with an account and a category", ARegisteredUserWithAnAccountAndACategory)
            .And("a purchase without a date", APurchaseWithoutADate)
            .When("the user records the purchase", RecordingThePurchase)
            .Then<HttpResponseMessage>("the request is rejected", TheRequestIsRejected)
            .PlayAsync();

    private async Task ARegisteredUserWithAnAccountAndACategory()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        var budget = new BudgetForTest(_user, server.Database);
        _chequing = await budget.AnAccountAsync("Chequing", AccountType.Checking);
        _groceries = await budget.ACategoryAsync(CategoryType.Expense, name: "Groceries");
    }

    private void AMonthlyPurchase() =>
        _body = new
        {
            date = "2026-11-11",
            amount = "42.50",
            description = "Market",
            accountId = _chequing.ToString(),
            categoryId = _groceries.ToString(),
            tags = new[] { "Weekly" },
            repeat = new { startDate = "2026-11-11", intervalType = "monthly", frequency = "1" }
        };

    private async Task APurchaseOnAnotherUsersAccount()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        var others = await new BudgetForTest(otherUser, server.Database).AnAccountAsync("Other", AccountType.Checking);
        _body = new { date = "2026-11-11", amount = "42.50", accountId = others.ToString(), categoryId = _groceries.ToString() };
    }

    private void APurchaseWithoutADate() =>
        _body = new { amount = "42.50", accountId = _chequing.ToString(), categoryId = _groceries.ToString() };

    private async Task<HttpResponseMessage> RecordingThePurchase()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(new Uri("/api/v2/transactions/record-purchase", UriKind.Relative), _body);
    }

    private async Task ThePurchaseAndItsCashflowAreRecorded(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Value.Content.ReadFromJsonAsync<CreatedJson>();
        var cashflow = (await server.Database.CashflowsInDbAsync(_chequing)).Should().ContainSingle().Subject;
        cashflow.Should().Be(new CashflowInDb(
            cashflow.Id, new DateOnly(2026, 11, 11), 42.50m, "Monthly", 1, 0, "Market", _chequing, _groceries, false, "weekly", _user.Id.Value));
        (await server.Database.TransactionsInDbAsync(_chequing)).Should().Equal(new TransactionInDb(
            created!.Id, new DateOnly(2026, 11, 11), 42.50m, "Market", _chequing, _groceries, cashflow.Id, new DateOnly(2026, 11, 11),
            "weekly", _user.Id.Value));
    }

    private static void TheAccountIsNotFound(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static void TheRequestIsRejected(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private sealed record CreatedJson(Guid Id);
}
