using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Cashflows.Api;
using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Cashflows.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.ServiceTests.Cashflows.Api.ClearCashflow;

[Collection(ComponentTestMarker.Name)]
public class ClearCashflowComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private const string ValidationErrorTitle = "One or more validation errors occurred.";
    private static readonly DateOnly Scheduled = new(2026, 11, 1);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Cashflow _cashflow = null!;

    [Fact]
    public Task WhenClearingAnOccurrence() =>
        ScenarioRunner.Create("Clearing an upcoming cashflow", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a cashflow", TheUserHasACashflow)
            .When("the user clears its occurrence", () => Clearing(_cashflow.Id.Value, "2026-11-01"))
            .Then<HttpResponseMessage>("the payment id is returned", ThePaymentIdIsReturned)
            .And("a zero payment is recorded on the scheduled date", AZeroPaymentIsRecorded)
            .PlayAsync();

    [Fact]
    public Task WhenTheCashflowDateIsInvalid() =>
        ScenarioRunner.Create("Clearing an upcoming cashflow with an invalid date", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a cashflow", TheUserHasACashflow)
            .When("the user clears it without a date", () => Clearing(_cashflow.Id.Value, null))
            .Then<HttpResponseMessage>("a validation error is returned on the cashflow date",
                response => AValidationErrorIsReturnedOn(response, IsoDateErrors.CodeCashflowDateInvalid))
            .PlayAsync();

    [Fact]
    public Task WhenTheCashflowBelongsToAnotherUser() =>
        ScenarioRunner.Create("Clearing another user's cashflow", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("another user has a cashflow", AnotherUserHasACashflow)
            .When("the user clears the other user's cashflow", () => Clearing(_cashflow.Id.Value, "2026-11-01"))
            .Then<HttpResponseMessage>("the cashflow is not found", TheCashflowIsNotFound)
            .And("no payment is recorded", NoPaymentIsRecorded)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Clearing a cashflow as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .And("another user has a cashflow", AnotherUserHasACashflow)
            .When("the user clears the other user's cashflow", () => Clearing(_cashflow.Id.Value, "2026-11-01"))
            .Then<HttpResponseMessage>("the cashflow is not found", TheCashflowIsNotFound)
            .And("no payment is recorded", NoPaymentIsRecorded)
            .PlayAsync();

    private async Task ARegisteredUser() => _user = await AUser().SavedInDbAsync(server.Database);

    private void AnUnregisteredUser() => _user = AUser();

    private async Task TheUserHasACashflow() =>
        _cashflow = await ACashflow(amount: 1450m).SavedInDbAsync(_user, server.Database);

    private async Task AnotherUserHasACashflow()
    {
        var owner = await AUser().SavedInDbAsync(server.Database);
        _cashflow = await ACashflow(amount: 1450m).SavedInDbAsync(owner, server.Database);
    }

    private async Task<HttpResponseMessage> Clearing(Guid cashflowId, string? cashflowDate)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(
            new Uri("/api/v2/cashflows/clear-cashflow", UriKind.Relative),
            new { CashflowId = cashflowId.ToString(), CashflowDate = cashflowDate });
    }

    private Guid _paymentId;

    private async Task ThePaymentIdIsReturned(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Value.Content.ReadFromJsonAsync<PaymentResponse>();
        body.Should().NotBeNull();
        _paymentId = body.Id;
    }

    private async Task AZeroPaymentIsRecorded() =>
        (await server.Database.PaymentsInDbAsync(_cashflow.Id)).Should().Equal(
            _cashflow.Pay(Id.Valid(_paymentId), Scheduled, Money.Zero, Scheduled));

    private async Task NoPaymentIsRecorded() =>
        (await server.Database.PaymentsInDbAsync(_cashflow.Id)).Should().BeEmpty();

    private static async Task AValidationErrorIsReturnedOn(Ensure<HttpResponseMessage> response, string errorCode)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Value.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Title.Should().Be(ValidationErrorTitle);
        problem.Errors.Should().ContainKey(errorCode);
    }

    private static async Task TheCashflowIsNotFound(Ensure<HttpResponseMessage> response)
    {
        response.Should().BeValid();
        response.Value.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Value.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Detail.Should().Be(CashflowErrors.NotFound.Description);
    }

    private sealed record PaymentResponse(Guid Id);
}
