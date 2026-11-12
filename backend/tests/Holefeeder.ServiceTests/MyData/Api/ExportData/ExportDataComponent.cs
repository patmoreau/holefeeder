using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.MyData.Api.ExportData;

[Collection(ComponentTestMarker.Name)]
public class ExportDataComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _accountId;
    private Guid _categoryId;
    private Guid _cashflowId;
    private Guid _transactionId;

    [Fact]
    public Task WhenTheUserHasData() =>
        ScenarioRunner.Create("Exporting the user's data", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has an account, a category, a cashflow and a transaction", TheUserHasData)
            .And("another user has data", AnotherUserHasData)
            .When("the user exports their data", ExportingTheirData)
            .Then<HttpResponseMessage>("only the user's data is exported", TheUsersDataIsExported)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserHasNoData() =>
        ScenarioRunner.Create("Exporting without data", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user exports their data", ExportingTheirData)
            .Then<HttpResponseMessage>("empty collections are exported", NothingIsExported)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Exporting as an unregistered user", _scenarioOutput)
            .Given("an authenticated user who is not registered", () => _user = AUser())
            .When("the user exports their data", ExportingTheirData)
            .Then<HttpResponseMessage>("empty collections are exported", NothingIsExported)
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private async Task TheUserHasData()
    {
        var budget = new BudgetForTest(_user, server.Database);
        _accountId = await budget.AnAccountAsync("Visa", AccountType.CreditCard, openBalance: 12.5m, openDate: new DateOnly(2021, 2, 3),
            favorite: true, description: "Travel");
        _categoryId = await budget.ACategoryAsync(CategoryType.Gain, name: "Salary", color: "#00FF00", favorite: true, budget: 2500m);
        _cashflowId = await budget.ACashflowAsync(_categoryId, new DateOnly(2024, 1, 15), IntervalType.OneTime, 1, 1500m,
            accountId: _accountId, description: "Bonus", tags: "work,yearly");
        _transactionId = await budget.ATransactionAsync(_categoryId, new DateOnly(2024, 5, 17), 42.99m, _accountId, "Lunch", "food");
    }

    private async Task AnotherUserHasData()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        var budget = new BudgetForTest(otherUser, server.Database);
        var categoryId = await budget.ACategoryAsync(CategoryType.Expense);
        await budget.ATransactionAsync(categoryId, new DateOnly(2024, 1, 1), 1m);
    }

    private async Task<HttpResponseMessage> ExportingTheirData()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri("/api/v2/my-data/export-data", UriKind.Relative));
    }

    private async Task TheUsersDataIsExported(Ensure<HttpResponseMessage> response)
    {
        var exported = await ExportedDataOf(response);

        exported.Accounts.Should().BeEquivalentTo([
            new ExportedAccount(_accountId, "creditCard", "Visa", 12.5m, "2021-02-03", "Travel", Favorite: true, Inactive: false)
        ]);
        exported.Categories.Should().BeEquivalentTo([
            new ExportedCategory(_categoryId, "gain", "Salary", "#00FF00", 2500m, Favorite: true, System: false, Inactive: false)
        ]);
        exported.Cashflows.Should().BeEquivalentTo([
            new ExportedCashflow(_cashflowId, "2024-01-15", 1500m, "onetime", 1, 0, "Bonus", ["work", "yearly"], _categoryId, _accountId,
                Inactive: false)
        ]);
        exported.Transactions.Should().BeEquivalentTo([
            new ExportedTransaction(_transactionId, "2024-05-17", 42.99m, "Lunch", ["food"], _categoryId, _accountId, null, null)
        ]);
    }

    private static async Task NothingIsExported(Ensure<HttpResponseMessage> response)
    {
        var exported = await ExportedDataOf(response);

        exported.Should().BeEquivalentTo(new ExportedData([], [], [], []));
    }

    private static async Task<ExportedData> ExportedDataOf(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.OK);
        var exported = await response.Value.Content.ReadFromJsonAsync<ExportedData>();
        exported.Should().NotBeNull();
        return exported;
    }

    private sealed record ExportedData(
        ExportedAccount[] Accounts,
        ExportedCategory[] Categories,
        ExportedCashflow[] Cashflows,
        ExportedTransaction[] Transactions);

    private sealed record ExportedAccount(
        Guid Id, string Type, string Name, decimal OpenBalance, string OpenDate, string Description, bool Favorite, bool Inactive);

    private sealed record ExportedCategory(
        Guid Id, string Type, string Name, string Color, decimal BudgetAmount, bool Favorite, bool System, bool Inactive);

    private sealed record ExportedCashflow(
        Guid Id,
        string EffectiveDate,
        decimal Amount,
        string IntervalType,
        int Frequency,
        int Recurrence,
        string Description,
        string[] Tags,
        Guid CategoryId,
        Guid AccountId,
        bool Inactive);

    private sealed record ExportedTransaction(
        Guid Id,
        string Date,
        decimal Amount,
        string Description,
        string[] Tags,
        Guid CategoryId,
        Guid AccountId,
        Guid? CashflowId,
        string? CashflowDate);
}
