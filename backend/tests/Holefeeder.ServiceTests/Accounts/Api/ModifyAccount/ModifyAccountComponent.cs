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

namespace Holefeeder.ServiceTests.Accounts.Api.ModifyAccount;

[Collection(ComponentTestMarker.Name)]
public class ModifyAccountComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private const string ValidationErrorTitle = "One or more validation errors occurred.";
    private static readonly DateOnly OpenDate = new(2025, 3, 1);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Guid _amex;

    [Fact]
    public Task WhenTheIdIsEmpty() =>
        ScenarioRunner.Create("Modifying an account with an empty id", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user modifies an account with an empty id", () => ModifyingTheAccount(Guid.Empty, "Amex Gold"))
            .Then<HttpResponseMessage>("a validation error is returned on the id", AValidationErrorIsReturnedOnTheId)
            .PlayAsync();

    [Fact]
    public Task WhenTheAccountBelongsToAnotherUser() =>
        ScenarioRunner.Create("Modifying another user's account", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("another user has a credit card", AnotherUserHasACreditCard)
            .When("the user modifies the other user's credit card", () => ModifyingTheAccount(_amex, "Amex Gold"))
            .Then<HttpResponseMessage>("the account is not found", TheAccountIsNotFound)
            .PlayAsync();

    [Fact]
    public Task WhenTheNameBelongsToAnotherAccount() =>
        ScenarioRunner.Create("Renaming an account to the name of another account", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a credit card", TheUserHasACreditCard)
            .And("the user has a chequing account", TheUserHasAChequingAccount)
            .When("the user renames the credit card to Chequing", () => ModifyingTheAccount(_amex, "Chequing"))
            .Then<HttpResponseMessage>("the name is reported as already existing", TheNameIsReportedAsAlreadyExisting)
            .PlayAsync();

    [Fact]
    public Task WhenTheAccountIsModified() =>
        ScenarioRunner.Create("Modifying an account", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a credit card", TheUserHasACreditCard)
            .When("the user renames the credit card", () => ModifyingTheAccount(_amex, "Amex Gold"))
            .Then<HttpResponseMessage>("the account is modified", TheAccountIsModified)
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private async Task TheUserHasACreditCard() =>
        _amex = await new BudgetForTest(_user, server.Database).AnAccountAsync("Amex", AccountType.CreditCard, openDate: OpenDate,
            favorite: true);

    private async Task TheUserHasAChequingAccount() =>
        await new BudgetForTest(_user, server.Database).AnAccountAsync("Chequing", AccountType.Checking);

    private async Task AnotherUserHasACreditCard()
    {
        var otherUser = await AUser().SavedInDbAsync(server.Database);
        _amex = await new BudgetForTest(otherUser, server.Database).AnAccountAsync("Amex", AccountType.CreditCard);
    }

    private async Task<HttpResponseMessage> ModifyingTheAccount(Guid id, string name)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(
            new Uri("/api/v2/accounts/modify-account", UriKind.Relative),
            new { Id = id, Name = name, OpenBalance = 99.5m, Description = "Travel card" });
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

    private static async Task TheNameIsReportedAsAlreadyExisting(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(AccountNameErrors.AlreadyExists.Description);
    }

    private async Task TheAccountIsModified(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);

        using var client = server.CreateClientFor(_user.IdentityObjectId);
        var account = await client.GetFromJsonAsync<ModifiedAccountJson>(new Uri($"/api/v2/accounts/{_amex}/detail", UriKind.Relative));
        account.Should().Be(new ModifiedAccountJson(_amex, "Amex Gold", "CreditCard", "Travel card", 99.5m, OpenDate, Favorite: true,
            Inactive: false));
    }

    private sealed record ModifiedAccountJson(
        Guid Id,
        string Name,
        string Type,
        string Description,
        decimal OpenBalance,
        DateOnly OpenDate,
        bool Favorite,
        bool Inactive);
}
