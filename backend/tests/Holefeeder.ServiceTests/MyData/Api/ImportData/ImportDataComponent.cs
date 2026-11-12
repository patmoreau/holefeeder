using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

namespace Holefeeder.ServiceTests.MyData.Api.ImportData;

[Collection(ComponentTestMarker.Name)]
public class ImportDataComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private const string ValidationErrorTitle = "One or more validation errors occurred.";
    private static readonly TimeSpan ImportTimeout = TimeSpan.FromSeconds(60);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _accountId;
    private Guid _categoryId;
    private JsonObject _body = null!;
    private Guid _importId;

    [Fact]
    public Task WhenTheDataIsImported() =>
        ScenarioRunner.Create("Importing data", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request with an account, a category, a cashflow and a transaction", AFullRequest)
            .When("the user imports their data", ImportingTheirData)
            .Then<HttpResponseMessage>("the import is accepted", TheImportIsAccepted)
            .And("the import completes", TheImportCompletes)
            .And("the data is the user's", TheDataIsTheUsers)
            .PlayAsync();

    [Fact]
    public Task WhenTheRecordsExistAndAreNotUpdated() =>
        ScenarioRunner.Create("Importing over existing data without updating", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has an account", TheUserHasAnAccount)
            .And("a request renaming the account", () => _body = ABody(updateExisting: false, accounts: [AnAccount(_accountId, "Renamed")]))
            .When("the user imports their data", ImportingTheirData)
            .Then<HttpResponseMessage>("the import is accepted", TheImportIsAccepted)
            .And("the import completes", TheImportCompletes)
            .And("the account keeps its name", () => TheAccountIsNamed("Chequing"))
            .PlayAsync();

    [Fact]
    public Task WhenTheRecordsExistAndAreUpdated() =>
        ScenarioRunner.Create("Importing over existing data while updating", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has an account", TheUserHasAnAccount)
            .And("a request renaming the account", () => _body = ABody(updateExisting: true, accounts: [AnAccount(_accountId, "Renamed")]))
            .When("the user imports their data", ImportingTheirData)
            .Then<HttpResponseMessage>("the import is accepted", TheImportIsAccepted)
            .And("the import completes", TheImportCompletes)
            .And("the account is renamed", () => TheAccountIsNamed("Renamed"))
            .PlayAsync();

    [Fact]
    public Task WhenTheImportFails() =>
        ScenarioRunner.Create("Importing a transaction of an unknown account", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request with a transaction of an unknown account", () => _body = ABody(updateExisting: false,
                transactions: [ATransaction(Guid.NewGuid(), Guid.NewGuid())]))
            .When("the user imports their data", ImportingTheirData)
            .Then<HttpResponseMessage>("the import is accepted", TheImportIsAccepted)
            .And("the import ends in error", TheImportEndsInError)
            .PlayAsync();

    [Fact]
    public Task WhenThereIsNoData() =>
        ScenarioRunner.Create("Importing no data", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request without data", () => _body = ABody(updateExisting: false))
            .When("the user imports their data", ImportingTheirData)
            .Then<HttpResponseMessage>("a validation error is returned", AValidationErrorIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenARecordIsInvalid() =>
        ScenarioRunner.Create("Importing an account without a name", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request with an account without a name", () => _body = ABody(updateExisting: false,
                accounts: [AnAccount(Guid.NewGuid(), " ")]))
            .When("the user imports their data", ImportingTheirData)
            .Then<HttpResponseMessage>("a validation error is returned", AValidationErrorIsReturned)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Importing as an unregistered user", _scenarioOutput)
            .Given("an authenticated user who is not registered", () => _user = AUser())
            .And("a request with an account", () => _body = ABody(updateExisting: false, accounts: [AnAccount(Guid.NewGuid(), "Visa")]))
            .When("the user imports their data", ImportingTheirData)
            .Then<HttpResponseMessage>("a bad request is returned", response =>
            {
                response.Should().BeValid();
                response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            })
            .PlayAsync();

    [Fact]
    public Task WhenTheStatusIsUnknown() =>
        ScenarioRunner.Create("Getting the status of an unknown import", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user gets the status of an unknown import", () => GettingTheStatus(Guid.NewGuid()))
            .Then<HttpResponseMessage>("the import is not found", TheImportIsNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenTheStatusBelongsToAnotherUser() =>
        ScenarioRunner.Create("Getting the status of another user's import", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("a request with an account", () => _body = ABody(updateExisting: false, accounts: [AnAccount(Guid.NewGuid(), "Visa")]))
            .And("the user imports their data", ImportingAndKeepingTheId)
            .When("another user gets the status of that import", GettingTheStatusAsAnotherUser)
            .Then<HttpResponseMessage>("the import is not found", TheImportIsNotFound)
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private async Task TheUserHasAnAccount() =>
        _accountId = await new BudgetForTest(_user, server.Database).AnAccountAsync("Chequing", AccountType.Checking);

    private void AFullRequest()
    {
        (_accountId, _categoryId) = (Guid.NewGuid(), Guid.NewGuid());
        _body = ABody(updateExisting: false,
            accounts: [AnAccount(_accountId, "Visa")],
            categories: [new JsonObject
            {
                ["id"] = _categoryId, ["type"] = "gain", ["name"] = "Salary", ["color"] = "#00FF00", ["budgetAmount"] = 2500,
                ["favorite"] = true, ["system"] = false, ["inactive"] = false
            }],
            cashflows: [new JsonObject
            {
                ["id"] = Guid.NewGuid(), ["effectiveDate"] = "2024-01-15", ["amount"] = 1500, ["intervalType"] = "monthly",
                ["frequency"] = 1, ["recurrence"] = 0, ["description"] = "Rent", ["tags"] = new JsonArray("home"),
                ["categoryId"] = _categoryId, ["accountId"] = _accountId, ["inactive"] = false
            }],
            transactions: [ATransaction(_accountId, _categoryId)]);
    }

    private static JsonObject AnAccount(Guid id, string name) =>
        new()
        {
            ["id"] = id,
            ["type"] = "creditCard",
            ["name"] = name,
            ["openBalance"] = 12.5,
            ["openDate"] = "2021-02-03",
            ["description"] = "Travel",
            ["favorite"] = true,
            ["inactive"] = false
        };

    private static JsonObject ATransaction(Guid accountId, Guid categoryId) =>
        new()
        {
            ["id"] = Guid.NewGuid(),
            ["date"] = "2024-05-17",
            ["amount"] = 42.99,
            ["description"] = "Lunch",
            ["tags"] = new JsonArray("food"),
            ["categoryId"] = categoryId,
            ["accountId"] = accountId
        };

    private static JsonObject ABody(
        bool updateExisting,
        JsonObject[]? accounts = null,
        JsonObject[]? categories = null,
        JsonObject[]? cashflows = null,
        JsonObject[]? transactions = null) =>
        new()
        {
            ["updateExisting"] = updateExisting,
            ["data"] = new JsonObject
            {
                ["accounts"] = new JsonArray([.. accounts ?? []]),
                ["categories"] = new JsonArray([.. categories ?? []]),
                ["cashflows"] = new JsonArray([.. cashflows ?? []]),
                ["transactions"] = new JsonArray([.. transactions ?? []])
            }
        };

    private async Task<HttpResponseMessage> ImportingTheirData()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(new Uri("/api/v2/my-data/import-data", UriKind.Relative), _body);
    }

    private async Task ImportingAndKeepingTheId()
    {
        using var response = await ImportingTheirData();
        _importId = (await response.Content.ReadFromJsonAsync<ImportResponse>())!.Id;
    }

    private async Task<HttpResponseMessage> GettingTheStatus(Guid importId)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.GetAsync(new Uri($"/api/v2/my-data/import-status/{importId}", UriKind.Relative));
    }

    private async Task<HttpResponseMessage> GettingTheStatusAsAnotherUser()
    {
        var other = await AUser().SavedInDbAsync(server.Database);
        using var client = server.CreateClientFor(other.IdentityObjectId);
        return await client.GetAsync(new Uri($"/api/v2/my-data/import-status/{_importId}", UriKind.Relative));
    }

    private async Task TheImportIsAccepted(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.Accepted);
        var accepted = await response.Value.Content.ReadFromJsonAsync<ImportResponse>();
        accepted.Should().NotBeNull();
        _importId = accepted.Id;
        response.Value.Headers.Location!.ToString().Should().EndWith($"/api/v2/my-data/import-status/{_importId}");
    }

    private async Task TheImportCompletes()
    {
        var status = await FinalStatusAsync();
        status.Status.Should().Be("Completed", status.Message);
    }

    private async Task TheImportEndsInError()
    {
        var status = await FinalStatusAsync();
        status.Status.Should().Be("Error");
        status.Message.Should().StartWith("my-data-write-failed");
    }

    private async Task<ImportStatusJson> FinalStatusAsync()
    {
        var deadline = DateTime.UtcNow + ImportTimeout;
        while (true)
        {
            using var response = await GettingTheStatus(_importId);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var status = (await response.Content.ReadFromJsonAsync<ImportStatusJson>())!;
            if (status.Status is "Completed" or "Error" || DateTime.UtcNow > deadline)
            {
                return status;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(250));
        }
    }

    private async Task TheDataIsTheUsers()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        var exported = await client.GetFromJsonAsync<JsonObject>(new Uri("/api/v2/my-data/export-data", UriKind.Relative));
        foreach (var table in new[] { "accounts", "categories", "cashflows", "transactions" })
        {
            exported![table]!.AsArray().Should().ContainSingle(table);
        }
        exported!["accounts"]![0]!["name"]!.GetValue<string>().Should().Be("Visa");
    }

    private async Task TheAccountIsNamed(string name)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        var exported = await client.GetFromJsonAsync<JsonObject>(new Uri("/api/v2/my-data/export-data", UriKind.Relative));
        exported!["accounts"]![0]!["name"]!.GetValue<string>().Should().Be(name);
    }

    private static async Task AValidationErrorIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<JsonObject>();
        problem!["title"]!.GetValue<string>().Should().Be(ValidationErrorTitle);
    }

    private static void TheImportIsNotFound(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private sealed record ImportResponse(Guid Id);

    private sealed record ImportStatusJson(
        string Status,
        int AccountsProcessed,
        int AccountsTotal,
        int CashflowsProcessed,
        int CashflowsTotal,
        int CategoriesProcessed,
        int CategoriesTotal,
        int TransactionsProcessed,
        int TransactionsTotal,
        string Message);
}
