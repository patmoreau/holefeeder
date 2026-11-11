using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Cashflows.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.ServiceTests.Cashflows.Api.CancelCashflow;

[Collection(ComponentTestMarker.Name)]
public class CancelCashflowComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Cashflow _cashflow = null!;

    [Fact]
    public Task WhenCancellingACashflow() =>
        ScenarioRunner.Create("Cancelling a cashflow", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a cashflow", TheUserHasACashflow)
            .When("the user cancels it", Cancelling)
            .Then<HttpResponseMessage>("no content is returned", NoContentIsReturned)
            .And("the cashflow is inactive", () => TheCashflowIsInactive(true))
            .PlayAsync();

    [Fact]
    public Task WhenTheCashflowIsAlreadyInactive() =>
        ScenarioRunner.Create("Cancelling a cashflow already cancelled", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a cancelled cashflow", TheUserHasACancelledCashflow)
            .When("the user cancels it", Cancelling)
            .Then<HttpResponseMessage>("the cashflow is already inactive", TheCashflowIsAlreadyInactive)
            .PlayAsync();

    [Fact]
    public Task WhenTheCashflowBelongsToAnotherUser() =>
        ScenarioRunner.Create("Cancelling another user's cashflow", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("another user has a cashflow", AnotherUserHasACashflow)
            .When("the user cancels the other user's cashflow", Cancelling)
            .Then<HttpResponseMessage>("the cashflow is not found", TheCashflowIsNotFound)
            .And("the cashflow stays active", () => TheCashflowIsInactive(false))
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Cancelling a cashflow as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .And("another user has a cashflow", AnotherUserHasACashflow)
            .When("the user cancels the other user's cashflow", Cancelling)
            .Then<HttpResponseMessage>("the cashflow is not found", TheCashflowIsNotFound)
            .And("the cashflow stays active", () => TheCashflowIsInactive(false))
            .PlayAsync();

    private async Task ARegisteredUser() => _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserHasACashflow() => _cashflow = await ACashflow().SavedInDbAsync(_user, server.Database);

    private async Task TheUserHasACancelledCashflow() => _cashflow = await ACashflow(inactive: true).SavedInDbAsync(_user, server.Database);

    private async Task AnotherUserHasACashflow()
    {
        var owner = await AUser().SavedInDbAsync(server.Database);
        _cashflow = await ACashflow().SavedInDbAsync(owner, server.Database);
    }

    private async Task<HttpResponseMessage> Cancelling()
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(
            new Uri("/api/v2/cashflows/cancel-cashflow", UriKind.Relative),
            new { CashflowId = _cashflow.Id.Value.ToString() });
    }

    private static void NoContentIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private async Task TheCashflowIsInactive(bool inactive) =>
        (await server.Database.CashflowInDbAsync(_cashflow.Id))!.Inactive.Should().Be(inactive);

    private static async Task TheCashflowIsAlreadyInactive(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(CashflowErrors.AlreadyInactive.Description);
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
