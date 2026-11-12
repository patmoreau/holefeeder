using System.Net;
using System.Net.Http.Json;

using Dapper;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

using Microsoft.AspNetCore.Http;

namespace Holefeeder.ServiceTests.Sync.Api.SyncData;

[Collection(ComponentTestMarker.Name)]
public class SyncDataComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private const string ValidationErrorTitle = "One or more validation errors occurred.";

    private static readonly string[] CashflowTags = ["Home", "bills"];

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private BudgetForTest _budget = null!;
    private Guid _accountId;
    private Guid _categoryId;
    private Guid _rowId;
    private Dictionary<string, object?> _body = null!;

    [Fact]
    public Task WhenTheTransactionIdIsMissing() =>
        ScenarioRunner.Create("Syncing without a transaction id", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request without a transaction id", () => _body = ABody(0, AnOperation("accounts", "DELETE", Guid.NewGuid())))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("a validation error is returned", AValidationErrorIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenTheTypeIsUnknown() =>
        ScenarioRunner.Create("Syncing an unknown table", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request for an unknown table", () => _body = ABody(1, AnOperation("budgets", "DELETE", Guid.NewGuid())))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("a validation error is returned", AValidationErrorIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenTheOperationIsUnknown() =>
        ScenarioRunner.Create("Syncing an unknown operation", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request with an unknown operation", () => _body = ABody(1, AnOperation("accounts", "MERGE", Guid.NewGuid())))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("a validation error is returned", AValidationErrorIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenTheIdIsEmpty() =>
        ScenarioRunner.Create("Syncing an operation without an id", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request with an empty id", () => _body = ABody(1, AnOperation("accounts", "DELETE", Guid.Empty)))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("a validation error is returned", AValidationErrorIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenTheDataIsMalformed() =>
        ScenarioRunner.Create("Syncing malformed data", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request with a malformed date", () => _body = ABody(1, AnOperation("accounts", "PUT", Guid.NewGuid(),
                new Dictionary<string, object?> { ["name"] = "Chequing", ["open_date"] = "2024-13-45" })))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("a validation error is returned", AValidationErrorIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenThereAreNoOperations() =>
        ScenarioRunner.Create("Syncing no operations", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request with no operations", () => _body = ABody(1))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("no content is returned", NoContentIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenSyncingAnAccountPut() =>
        ScenarioRunner.Create("Syncing a new account", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request putting a new account", () =>
            {
                _rowId = Guid.NewGuid();
                _body = ABody(1, AnOperation("accounts", "PUT", _rowId, new Dictionary<string, object?>
                {
                    ["type"] = "Savings",
                    ["name"] = "Rainy day",
                    ["open_balance"] = 12345,
                    ["open_date"] = "2024-02-29",
                    ["description"] = "Emergency fund",
                    ["favorite"] = 1,
                    ["inactive"] = 0
                }));
            })
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the account is stored", response => TheRowIs(response,
                "SELECT type, name, open_balance, open_date, description, favorite, inactive FROM accounts WHERE id = @Id",
                new AccountRow("Savings", "Rainy day", 123.45m, new DateOnly(2024, 2, 29), "Emergency fund", true, false)))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingAnAccountPatch() =>
        ScenarioRunner.Create("Syncing an account change", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has an account", TheUserHasAnAccount)
            .And("a request patching the account", () => _body = ABody(1, AnOperation("accounts", "PATCH", _accountId,
                new Dictionary<string, object?> { ["name"] = "Updated via sync", ["description"] = "Patched description" })))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("only the patched fields change", response => TheRowIs(response,
                "SELECT type, name, open_balance, open_date, description, favorite, inactive FROM accounts WHERE id = @Id",
                new AccountRow("Checking", "Updated via sync", 50m, new DateOnly(2020, 1, 1), "Patched description", false, false),
                _accountId))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingAnAccountDelete() =>
        ScenarioRunner.Create("Syncing an account deletion", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has an account", TheUserHasAnAccount)
            .And("a request deleting the account", () => _body = ABody(1, AnOperation("accounts", "DELETE", _accountId)))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the account is gone", response => TheRowIsGone(response, "accounts", _accountId))
            .PlayAsync();

    [Fact]
    public Task WhenPatchingAnAccountThatDoesNotExist() =>
        ScenarioRunner.Create("Syncing a change to a missing account", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request patching a missing account", () => _body = ABody(1, AnOperation("accounts", "PATCH", Guid.NewGuid(),
                new Dictionary<string, object?> { ["name"] = "Ghost" })))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the account is not found", NotFoundIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenPatchingAnotherUsersAccount() =>
        ScenarioRunner.Create("Syncing a change to another user's account", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("another user has an account", AnotherUserHasAnAccount)
            .And("a request patching that account", () => _body = ABody(1, AnOperation("accounts", "PATCH", _accountId,
                new Dictionary<string, object?> { ["name"] = "Stolen" })))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the account is not found", NotFoundIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenALaterOperationFails() =>
        ScenarioRunner.Create("Syncing a batch whose last operation fails", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has an account", TheUserHasAnAccount)
            .And("a request deleting the account then patching a missing one", () => _body = ABody(1,
                AnOperation("accounts", "DELETE", _accountId),
                AnOperation("accounts", "PATCH", Guid.NewGuid(), new Dictionary<string, object?> { ["name"] = "Ghost" })))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("nothing is applied", NothingIsApplied)
            .PlayAsync();

    [Fact]
    public Task WhenSyncingACategoryPut() =>
        ScenarioRunner.Create("Syncing a new category", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request putting a new category", () =>
            {
                _rowId = Guid.NewGuid();
                _body = ABody(1, AnOperation("categories", "PUT", _rowId, new Dictionary<string, object?>
                {
                    ["type"] = "Gain",
                    ["name"] = "Salary",
                    ["color"] = "#00FF00",
                    ["budget_amount"] = 250000,
                    ["favorite"] = 1,
                    ["system"] = 0,
                    ["inactive"] = 0
                }));
            })
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the category is stored", response => TheRowIs(response,
                "SELECT type, name, color, budget_amount, favorite, system, inactive FROM categories WHERE id = @Id",
                new CategoryRow("Gain", "Salary", "#00FF00", 2500m, true, false, false)))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingACategoryPatch() =>
        ScenarioRunner.Create("Syncing a category change", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a category", TheUserHasACategory)
            .And("a request patching the category", () => _body = ABody(1, AnOperation("categories", "PATCH", _categoryId,
                new Dictionary<string, object?> { ["name"] = "Updated via sync", ["budget_amount"] = 4000 })))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("only the patched fields change", response => TheRowIs(response,
                "SELECT type, name, color, budget_amount, favorite, system, inactive FROM categories WHERE id = @Id",
                new CategoryRow("Expense", "Updated via sync", "#123456", 40m, false, false, false), _categoryId))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingACategoryDelete() =>
        ScenarioRunner.Create("Syncing a category deletion", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a category", TheUserHasACategory)
            .And("a request deleting the category", () => _body = ABody(1, AnOperation("categories", "DELETE", _categoryId)))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the category is made inactive", response => TheRowIs(response,
                "SELECT type, name, color, budget_amount, favorite, system, inactive FROM categories WHERE id = @Id",
                new CategoryRow("Expense", "Groceries", "#123456", 20m, false, false, true), _categoryId))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingAStoreItemPut() =>
        ScenarioRunner.Create("Syncing a new store item", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request putting a new store item", () =>
            {
                _rowId = Guid.NewGuid();
                _body = ABody(1, AnOperation("store_items", "PUT", _rowId,
                    new Dictionary<string, object?> { ["code"] = "preferences", ["data"] = "{\"theme\":\"dark\"}" }));
            })
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the store item is stored", response => TheRowIs(response,
                "SELECT code, data FROM store_items WHERE id = @Id", new StoreItemRow("preferences", "{\"theme\":\"dark\"}")))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingAStoreItemPutOverAnExistingItem() =>
        ScenarioRunner.Create("Syncing a put over an existing store item", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a store item", TheUserHasAStoreItem)
            .And("a request putting new data only", () => _body = ABody(1, AnOperation("store_items", "PUT", _rowId,
                new Dictionary<string, object?> { ["data"] = "Updated via sync" })))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the code is kept and the data replaced", response => TheRowIs(response,
                "SELECT code, data FROM store_items WHERE id = @Id", new StoreItemRow("reminders", "Updated via sync"), _rowId))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingAStoreItemDelete() =>
        ScenarioRunner.Create("Syncing a store item deletion", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a store item", TheUserHasAStoreItem)
            .And("a request deleting the store item", () => _body = ABody(1, AnOperation("store_items", "DELETE", _rowId)))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the store item is gone", response => TheRowIsGone(response, "store_items", _rowId))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingATransactionPut() =>
        ScenarioRunner.Create("Syncing a new transaction", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has an account and a category", TheUserHasAnAccountAndACategory)
            .And("a request putting a new transaction", () =>
            {
                _rowId = Guid.NewGuid();
                _body = ABody(1, AnOperation("transactions", "PUT", _rowId, new Dictionary<string, object?>
                {
                    ["date"] = "2024-05-17",
                    ["amount"] = 4299,
                    ["description"] = "Lunch",
                    ["account_id"] = _accountId.ToString(),
                    ["category_id"] = _categoryId.ToString(),
                    ["tags"] = "Food, work,food"
                }));
            })
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the transaction is stored", response => TheRowIs(response,
                "SELECT date, amount, description, account_id, category_id, cashflow_id, cashflow_date, tags FROM transactions WHERE id = @Id",
                new TransactionRow(new DateOnly(2024, 5, 17), 42.99m, "Lunch", _accountId, _categoryId, null, null, "food,work")))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingATransactionPatch() =>
        ScenarioRunner.Create("Syncing a transaction change", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a transaction", TheUserHasATransaction)
            .And("a request patching the transaction", () => _body = ABody(1, AnOperation("transactions", "PATCH", _rowId,
                new Dictionary<string, object?> { ["amount"] = 2000, ["description"] = "Updated via sync" })))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("only the patched fields change", response => TheRowIs(response,
                "SELECT date, amount, description, account_id, category_id, cashflow_id, cashflow_date, tags FROM transactions WHERE id = @Id",
                new TransactionRow(new DateOnly(2024, 3, 1), 20m, "Updated via sync", _accountId, _categoryId, null, null, "coffee,treat"),
                _rowId))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingATransactionPatchThatClearsTags() =>
        ScenarioRunner.Create("Syncing a transaction change that clears its tags", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a transaction", TheUserHasATransaction)
            .And("a request clearing the tags", () => _body = ABody(1, AnOperation("transactions", "PATCH", _rowId,
                new Dictionary<string, object?> { ["tags"] = string.Empty })))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the tags are cleared", response => TheRowIs(response,
                "SELECT date, amount, description, account_id, category_id, cashflow_id, cashflow_date, tags FROM transactions WHERE id = @Id",
                new TransactionRow(new DateOnly(2024, 3, 1), 10m, "Coffee", _accountId, _categoryId, null, null, string.Empty), _rowId))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingATransactionDelete() =>
        ScenarioRunner.Create("Syncing a transaction deletion", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a transaction", TheUserHasATransaction)
            .And("a request deleting the transaction", () => _body = ABody(1, AnOperation("transactions", "DELETE", _rowId)))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the transaction is gone", response => TheRowIsGone(response, "transactions", _rowId))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingACashflowPut() =>
        ScenarioRunner.Create("Syncing a new cashflow", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has an account and a category", TheUserHasAnAccountAndACategory)
            .And("a request putting a new cashflow", () =>
            {
                _rowId = Guid.NewGuid();
                _body = ABody(1, AnOperation("cashflows", "PUT", _rowId, new Dictionary<string, object?>
                {
                    ["effective_date"] = "2024-01-15",
                    ["interval_type"] = "Weekly",
                    ["frequency"] = 2,
                    ["recurrence"] = 0,
                    ["amount"] = 150000,
                    ["description"] = "Rent",
                    ["account_id"] = _accountId.ToString(),
                    ["category_id"] = _categoryId.ToString(),
                    ["inactive"] = 0,
                    ["tags"] = CashflowTags
                }));
            })
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the cashflow is stored", response => TheRowIs(response,
                "SELECT effective_date, interval_type, frequency, recurrence, amount, description, account_id, category_id, inactive, tags FROM cashflows WHERE id = @Id",
                new CashflowRow(new DateOnly(2024, 1, 15), "Weekly", 2, 0, 1500m, "Rent", _accountId, _categoryId, false, "home,bills")))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingACashflowPatch() =>
        ScenarioRunner.Create("Syncing a cashflow change", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a cashflow", TheUserHasACashflow)
            .And("a request patching the cashflow", () => _body = ABody(1, AnOperation("cashflows", "PATCH", _rowId,
                new Dictionary<string, object?> { ["amount"] = 9900, ["description"] = "Updated via sync" })))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("only the patched fields change", response => TheRowIs(response,
                "SELECT effective_date, interval_type, frequency, recurrence, amount, description, account_id, category_id, inactive, tags FROM cashflows WHERE id = @Id",
                new CashflowRow(new DateOnly(2024, 1, 1), "Monthly", 1, 0, 99m, "Updated via sync", _accountId, _categoryId, false, "phone"),
                _rowId))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingACashflowPatchThatClearsTags() =>
        ScenarioRunner.Create("Syncing a cashflow change that clears its tags", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a cashflow", TheUserHasACashflow)
            .And("a request clearing the tags", () => _body = ABody(1, AnOperation("cashflows", "PATCH", _rowId,
                new Dictionary<string, object?> { ["tags"] = null })))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the tags are cleared", response => TheRowIs(response,
                "SELECT effective_date, interval_type, frequency, recurrence, amount, description, account_id, category_id, inactive, tags FROM cashflows WHERE id = @Id",
                new CashflowRow(new DateOnly(2024, 1, 1), "Monthly", 1, 0, 45m, "Phone", _accountId, _categoryId, false, string.Empty), _rowId))
            .PlayAsync();

    [Fact]
    public Task WhenSyncingACashflowDelete() =>
        ScenarioRunner.Create("Syncing a cashflow deletion", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a cashflow", TheUserHasACashflow)
            .And("a request deleting the cashflow", () => _body = ABody(1, AnOperation("cashflows", "DELETE", _rowId)))
            .When("the user syncs their data", Syncing)
            .Then<HttpResponseMessage>("the cashflow is gone", response => TheRowIsGone(response, "cashflows", _rowId))
            .PlayAsync();

    private async Task ARegisteredUser()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        _budget = new BudgetForTest(_user, server.Database);
    }

    private async Task TheUserHasAnAccount() =>
        _accountId = await _budget.AnAccountAsync("Chequing", AccountType.Checking, openBalance: 50m);

    private async Task AnotherUserHasAnAccount()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        _accountId = await new BudgetForTest(otherUser, server.Database).AnAccountAsync("Theirs", AccountType.Checking);
    }

    private async Task TheUserHasACategory() =>
        _categoryId = await _budget.ACategoryAsync(CategoryType.Expense, name: "Groceries", color: "#123456", budget: 20m);

    private async Task TheUserHasAnAccountAndACategory()
    {
        await TheUserHasAnAccount();
        await TheUserHasACategory();
    }

    private async Task TheUserHasAStoreItem()
    {
        _rowId = Guid.NewGuid();
        await using var connection = server.Database.DataSource.CreateConnection();
        await connection.ExecuteAsync(
            "INSERT INTO store_items (id, code, data, user_id) VALUES (@Id, 'reminders', 'original', @UserId)",
            new { Id = _rowId, UserId = _user.Id.Value });
    }

    private async Task TheUserHasATransaction()
    {
        await TheUserHasAnAccountAndACategory();
        _rowId = await _budget.ATransactionAsync(_categoryId, new DateOnly(2024, 3, 1), 10m, _accountId, "Coffee", "coffee,treat");
    }

    private async Task TheUserHasACashflow()
    {
        await TheUserHasAnAccountAndACategory();
        _rowId = await _budget.ACashflowAsync(_categoryId, new DateOnly(2024, 1, 1), IntervalType.Monthly, 1, 45m,
            accountId: _accountId, description: "Phone", tags: "phone");
    }

    private async Task<HttpResponseMessage> Syncing()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(new Uri("/api/v2/sync/powersync", UriKind.Relative), _body);
    }

    private static Dictionary<string, object?> ABody(long transactionId, params Dictionary<string, object?>[] operations) =>
        new() { ["transaction_id"] = transactionId, ["operations"] = operations };

    private static Dictionary<string, object?> AnOperation(
        string type, string op, Guid id, Dictionary<string, object?>? data = null) =>
        new() { ["type"] = type, ["op"] = op, ["id"] = id, ["data"] = data ?? [] };

    private static async Task AValidationErrorIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Title.Should().Be(ValidationErrorTitle);
    }

    private static void NoContentIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private static void NotFoundIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task TheRowIs<TRow>(Ensure<HttpResponseMessage> response, string sql, TRow expected, Guid? id = null)
    {
        NoContentIsReturned(response);
        await using var connection = server.Database.DataSource.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<TRow>(sql, new { Id = id ?? _rowId });
        row.Should().BeEquivalentTo(expected);
    }

    private async Task TheRowIsGone(Ensure<HttpResponseMessage> response, string table, Guid id)
    {
        NoContentIsReturned(response);
        await using var connection = server.Database.DataSource.CreateConnection();
        (await connection.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {table} WHERE id = @Id", new { Id = id }))
            .Should().Be(0);
    }

    private async Task NothingIsApplied(Ensure<HttpResponseMessage> response)
    {
        NotFoundIsReturned(response);
        await using var connection = server.Database.DataSource.CreateConnection();
        (await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM accounts WHERE id = @Id", new { Id = _accountId }))
            .Should().Be(1);
    }

    private sealed record AccountRow(
        string Type, string Name, decimal OpenBalance, DateOnly OpenDate, string Description, bool Favorite, bool Inactive);

    private sealed record CategoryRow(
        string Type, string Name, string Color, decimal BudgetAmount, bool Favorite, bool System, bool Inactive);

    private sealed record StoreItemRow(string Code, string Data);

    private sealed record TransactionRow(
        DateOnly Date, decimal Amount, string Description, Guid AccountId, Guid CategoryId, Guid? CashflowId, DateOnly? CashflowDate,
        string Tags);

    private sealed record CashflowRow(
        DateOnly EffectiveDate, string IntervalType, int Frequency, int Recurrence, decimal Amount, string Description, Guid AccountId,
        Guid CategoryId, bool Inactive, string Tags);
}
