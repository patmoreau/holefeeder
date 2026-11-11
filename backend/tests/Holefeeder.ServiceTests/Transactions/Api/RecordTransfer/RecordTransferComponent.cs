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

namespace Holefeeder.ServiceTests.Transactions.Api.RecordTransfer;

[Collection(ComponentTestMarker.Name)]
public class RecordTransferComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _chequing;
    private Guid _savings;
    private Guid _transferOut;
    private Guid _transferIn;
    private object _body = null!;

    [Fact]
    public Task WhenMovingMoneyBetweenTwoAccounts() =>
        ScenarioRunner.Create("Recording a transfer", _scenarioOutput)
            .Given("a registered user with two accounts", ARegisteredUserWithTwoAccounts)
            .And("a transfer from chequing to savings without a description", ATransferFromChequingToSavings)
            .When("the user records the transfer", RecordingTheTransfer)
            .Then<HttpResponseMessage>("the money leaves chequing and reaches savings", TheMoneyLeavesChequingAndReachesSavings)
            .PlayAsync();

    [Fact]
    public Task WhenBothAccountsAreTheSame() =>
        ScenarioRunner.Create("Recording a transfer to the same account", _scenarioOutput)
            .Given("a registered user with two accounts", ARegisteredUserWithTwoAccounts)
            .And("a transfer from chequing to chequing", ATransferFromChequingToChequing)
            .When("the user records the transfer", RecordingTheTransfer)
            .Then<HttpResponseMessage>("the request is rejected", TheRequestIsRejected)
            .PlayAsync();

    private async Task ARegisteredUserWithTwoAccounts()
    {
        _user = await AUser().SavedInDbAsync(server.Database);
        var budget = new BudgetForTest(_user, server.Database);
        _chequing = await budget.AnAccountAsync("Chequing", AccountType.Checking);
        _savings = await budget.AnAccountAsync("Savings", AccountType.Savings);
        _transferOut = await budget.ACategoryAsync(CategoryType.Expense, system: true, name: "Transfer Out");
        _transferIn = await budget.ACategoryAsync(CategoryType.Gain, system: true, name: "Transfer In");
    }

    private void ATransferFromChequingToSavings() =>
        _body = new { date = "2026-11-11", amount = "500", fromAccountId = _chequing.ToString(), toAccountId = _savings.ToString() };

    private void ATransferFromChequingToChequing() =>
        _body = new { date = "2026-11-11", amount = "500", fromAccountId = _chequing.ToString(), toAccountId = _chequing.ToString() };

    private async Task<HttpResponseMessage> RecordingTheTransfer()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(new Uri("/api/v2/transactions/record-transfer", UriKind.Relative), _body);
    }

    private async Task TheMoneyLeavesChequingAndReachesSavings(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Value.Content.ReadFromJsonAsync<CreatedJson>();
        const string description = "Transfer from 'Chequing' to 'Savings'";
        (await server.Database.TransactionsInDbAsync(_chequing)).Should().Equal(new TransactionInDb(
            created!.FromTransactionId, new DateOnly(2026, 11, 11), 500m, description, _chequing, _transferOut, null, null, "", _user.Id.Value));
        (await server.Database.TransactionsInDbAsync(_savings)).Should().Equal(new TransactionInDb(
            created.ToTransactionId, new DateOnly(2026, 11, 11), 500m, description, _savings, _transferIn, null, null, "", _user.Id.Value));
    }

    private static void TheRequestIsRejected(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private sealed record CreatedJson(Guid FromTransactionId, Guid ToTransactionId);
}
