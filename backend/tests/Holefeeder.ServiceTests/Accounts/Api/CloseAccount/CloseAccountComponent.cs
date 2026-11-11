using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Accounts.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;
using Holefeeder.ServiceTests.TestKit.Persistence;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.ServiceTests.Accounts.Api.CloseAccount;

[Collection(ComponentTestMarker.Name)]
public class CloseAccountComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private const string ValidationErrorTitle = "One or more validation errors occurred.";

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _amex;

    [Fact]
    public Task WhenTheIdIsEmpty() =>
        ScenarioRunner.Create("Closing an account with an empty id", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user closes an account with an empty id", () => ClosingTheAccount(Guid.Empty))
            .Then<HttpResponseMessage>("a validation error is returned on the id", AValidationErrorIsReturnedOnTheId)
            .PlayAsync();

    [Fact]
    public Task WhenTheAccountBelongsToAnotherUser() =>
        ScenarioRunner.Create("Closing another user's account", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("another user has a credit card", AnotherUserHasACreditCard)
            .When("the user closes the other user's credit card", () => ClosingTheAccount(_amex))
            .Then<HttpResponseMessage>("the account is not found", TheAccountIsNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenTheAccountIsAlreadyClosed() =>
        ScenarioRunner.Create("Closing an account that is already closed", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a closed credit card", () => TheUserHasACreditCard(inactive: true))
            .When("the user closes the credit card", () => ClosingTheAccount(_amex))
            .Then<HttpResponseMessage>("the account is reported as already closed", TheAccountIsReportedAsAlreadyClosed)
            .PlayAsync();

    [Fact]
    public Task WhenTheAccountHasAnActiveCashflow() =>
        ScenarioRunner.Create("Closing an account with an active cashflow", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a credit card", () => TheUserHasACreditCard(inactive: false))
            .And("the credit card has an active cashflow", TheCreditCardHasAnActiveCashflow)
            .When("the user closes the credit card", () => ClosingTheAccount(_amex))
            .Then<HttpResponseMessage>("the account is reported as having active cashflows", TheAccountIsReportedAsHavingActiveCashflows)
            .PlayAsync();

    [Fact]
    public Task WhenTheAccountIsOpen() =>
        ScenarioRunner.Create("Closing an account", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a credit card", () => TheUserHasACreditCard(inactive: false))
            .When("the user closes the credit card", () => ClosingTheAccount(_amex))
            .Then<HttpResponseMessage>("the account is closed", TheAccountIsClosed)
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private async Task TheUserHasACreditCard(bool inactive) =>
        _amex = await new BudgetForTest(_user, server.Database).AnAccountAsync("Amex", AccountType.CreditCard, inactive: inactive);

    private async Task TheCreditCardHasAnActiveCashflow()
    {
        var budget = new BudgetForTest(_user, server.Database);
        var subscriptions = await budget.ACategoryAsync(CategoryType.Expense);
        await budget.ACashflowAsync(subscriptions, new DateOnly(2026, 11, 20), IntervalType.Monthly, 1, 15m, accountId: _amex);
    }

    private async Task AnotherUserHasACreditCard()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        _amex = await new BudgetForTest(otherUser, server.Database).AnAccountAsync("Amex", AccountType.CreditCard);
    }

    private async Task<HttpResponseMessage> ClosingTheAccount(Guid id)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(new Uri("/api/v2/accounts/close-account", UriKind.Relative), new { Id = id });
    }

    private static async Task AValidationErrorIsReturnedOnTheId(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Title.Should().Be(ValidationErrorTitle);
        problem.Errors.Should().ContainKey(IdErrors.CodeInvalid);
    }

    private static async Task TheAccountIsNotFound(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(AccountErrors.NotFound.Description);
    }

    private static async Task TheAccountIsReportedAsAlreadyClosed(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(AccountClosingErrors.AlreadyClosed.Description);
    }

    private static async Task TheAccountIsReportedAsHavingActiveCashflows(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(AccountClosingErrors.HasActiveCashflows.Description);
    }

    private async Task TheAccountIsClosed(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var client = server.CreateClientFor(_user.IdentityObjectId);
        var account = await client.GetFromJsonAsync<ClosedAccountJson>(new Uri($"/api/v2/accounts/{_amex}/detail", UriKind.Relative));
        account.Should().Be(new ClosedAccountJson(_amex, Inactive: true));
    }

    private sealed record ClosedAccountJson(Guid Id, bool Inactive);
}
