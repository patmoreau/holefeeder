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

namespace Holefeeder.ServiceTests.Accounts.Api.OpenAccount;

[Collection(ComponentTestMarker.Name)]
public class OpenAccountComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private const string ValidationErrorTitle = "One or more validation errors occurred.";
    private static readonly DateOnly OpenDate = new(2025, 3, 1);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;

    [Fact]
    public Task WhenTheNameIsEmpty() =>
        ScenarioRunner.Create("Opening an account with an empty name", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user opens an account with an empty name", () => OpeningAnAccount(string.Empty))
            .Then<HttpResponseMessage>("a validation error is returned on the name", AValidationErrorIsReturnedOnTheName)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserAlreadyHasTheName() =>
        ScenarioRunner.Create("Opening an account with a name the user already has", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a credit card named Amex", TheUserHasACreditCardNamedAmex)
            .When("the user opens another account named Amex", () => OpeningAnAccount("Amex"))
            .Then<HttpResponseMessage>("the name is reported as already existing",
                response => AProblemIsReturned(response, AccountNameErrors.AlreadyExists.Description))
            .PlayAsync();

    [Fact]
    public Task WhenTheNameIsNew() =>
        ScenarioRunner.Create("Opening an account", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .When("the user opens a credit card named Amex", () => OpeningAnAccount("Amex"))
            .Then<HttpResponseMessage>("the account is opened", TheAccountIsOpened)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Opening an account as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .When("the user opens an account", () => OpeningAnAccount("Amex"))
            .Then<HttpResponseMessage>("the user is reported as not registered",
                response => AProblemIsReturned(response, UserErrors.NotRegistered.Description))
            .PlayAsync();

    private async Task ARegisteredUser() =>
        _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserHasACreditCardNamedAmex() =>
        await new BudgetForTest(_user, server.Database).AnAccountAsync("Amex", AccountType.CreditCard);

    private async Task<HttpResponseMessage> OpeningAnAccount(string name)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(
            new Uri("/api/v2/accounts/open-account", UriKind.Relative),
            new { Type = "creditCard", Name = name, OpenDate = "2025-03-01", OpenBalance = 250.75m, Description = "Travel card" });
    }

    private static async Task AValidationErrorIsReturnedOnTheName(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Title.Should().Be(ValidationErrorTitle);
        problem.Errors.Should().ContainKey(AccountNameErrors.CodeInvalid);
    }

    private static async Task AProblemIsReturned(Ensure<HttpResponseMessage> response, string detail)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(detail);
    }

    private async Task TheAccountIsOpened(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Value.Content.ReadFromJsonAsync<CreatedResponse>();
        created.Should().NotBeNull();
        response.Value.Headers.Location.Should().NotBeNull();
        response.Value.Headers.Location!.ToString().Should().EndWith($"/api/v2/accounts/{created.Id}");

        using var client = server.CreateClientFor(_user.IdentityObjectId);
        var account = await client.GetFromJsonAsync<OpenedAccountJson>(new Uri($"/api/v2/accounts/{created.Id}/detail", UriKind.Relative));
        account.Should().Be(new OpenedAccountJson(created.Id, "Amex", "CreditCard", "Travel card", 250.75m, OpenDate, Favorite: false,
            Inactive: false));
    }

    private sealed record CreatedResponse(Guid Id);

    private sealed record OpenedAccountJson(
        Guid Id,
        string Name,
        string Type,
        string Description,
        decimal OpenBalance,
        DateOnly OpenDate,
        bool Favorite,
        bool Inactive);
}
