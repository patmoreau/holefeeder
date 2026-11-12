using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Cashflows.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.ServiceTests.Cashflows.Api.ModifyCashflow;

[Collection(ComponentTestMarker.Name)]
public class ModifyCashflowComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private static readonly string[] SentTags = ["Home", "rent", "home"];

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Cashflow _cashflow = null!;

    [Fact]
    public Task WhenModifyingACashflow() =>
        ScenarioRunner.Create("Modifying a cashflow", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a cashflow", TheUserHasACashflow)
            .When("the user modifies it", () => Modifying(1500m, "2026-02-01"))
            .Then<HttpResponseMessage>("no content is returned", NoContentIsReturned)
            .And("the cashflow is modified", TheCashflowIsModified)
            .PlayAsync();

    [Fact]
    public Task WhenTheRequestIsInvalid() =>
        ScenarioRunner.Create("Modifying a cashflow with an invalid request", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a cashflow", TheUserHasACashflow)
            .When("the user modifies it with a negative amount and no date", () => Modifying(-5m, null))
            .Then<HttpResponseMessage>("a validation problem is returned", AValidationProblemIsReturned)
            .And("the cashflow is unchanged", TheCashflowIsUnchanged)
            .PlayAsync();

    [Fact]
    public Task WhenTheCashflowIsCancelled() =>
        ScenarioRunner.Create("Modifying a cancelled cashflow", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a cancelled cashflow", TheUserHasACancelledCashflow)
            .When("the user modifies it", () => Modifying(1500m, "2026-02-01"))
            .Then<HttpResponseMessage>("the cashflow cannot be changed", TheCashflowCannotBeChanged)
            .And("the cashflow is unchanged", TheCashflowIsUnchanged)
            .PlayAsync();

    [Fact]
    public Task WhenTheCashflowBelongsToAnotherUser() =>
        ScenarioRunner.Create("Modifying another user's cashflow", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("another user has a cashflow", AnotherUserHasACashflow)
            .When("the user modifies the other user's cashflow", () => Modifying(1500m, "2026-02-01"))
            .Then<HttpResponseMessage>("the cashflow is not found", TheCashflowIsNotFound)
            .And("the cashflow is unchanged", TheCashflowIsUnchanged)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Modifying a cashflow as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .And("another user has a cashflow", AnotherUserHasACashflow)
            .When("the user modifies the other user's cashflow", () => Modifying(1500m, "2026-02-01"))
            .Then<HttpResponseMessage>("the cashflow is not found", TheCashflowIsNotFound)
            .And("the cashflow is unchanged", TheCashflowIsUnchanged)
            .PlayAsync();

    private async Task ARegisteredUser() => _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserHasACashflow() => _cashflow = await ACashflow(1450m, description: "Rent", tags: "housing").SavedInDbAsync(_user, server.Database);

    private async Task TheUserHasACancelledCashflow() => _cashflow = await ACashflow(inactive: true).SavedInDbAsync(_user, server.Database);

    private async Task AnotherUserHasACashflow()
    {
        var owner = await AUser().SavedInDbAsync(server.Database);
        _cashflow = await ACashflow().SavedInDbAsync(owner, server.Database);
    }

    private async Task<HttpResponseMessage> Modifying(decimal amount, string? effectiveDate)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(
            new Uri("/api/v2/cashflows/modify", UriKind.Relative),
            new
            {
                Id = _cashflow.Id.Value,
                Amount = amount,
                EffectiveDate = effectiveDate,
                Description = "New rent",
                Tags = SentTags
            });
    }

    private static void NoContentIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task TheCashflowIsModified() =>
        (await server.Database.CashflowInDbAsync(_cashflow.Id)).Should().Be(_cashflow with
        {
            Amount = Money.Valid(1500m),
            EffectiveDate = new DateOnly(2026, 2, 1),
            Description = "New rent",
            Tags = "home,rent"
        });

    private async Task TheCashflowIsUnchanged() =>
        (await server.Database.CashflowInDbAsync(_cashflow.Id)).Should().Be(_cashflow);

    private static async Task AValidationProblemIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Errors.Should().ContainKeys(MoneyErrors.CodeInvalid, "cashflow-effective-date-invalid");
    }

    private static async Task TheCashflowCannotBeChanged(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(CashflowErrors.Cancelled.Description);
    }

    private static async Task TheCashflowIsNotFound(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(CashflowErrors.NotFound.Description);
    }
}
