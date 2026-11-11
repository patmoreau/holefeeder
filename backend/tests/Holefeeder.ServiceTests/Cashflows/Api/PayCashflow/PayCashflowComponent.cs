using System.Net;
using System.Net.Http.Json;

using DrifterApps.Seeds.FluentScenario;

using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Support.Core;
using Holefeeder.ServiceTests.Cashflows.TestKit;
using Holefeeder.ServiceTests.TestKit;
using Holefeeder.ServiceTests.TestKit.Api;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Holefeeder.ServiceTests.Cashflows.Api.PayCashflow;

[Collection(ComponentTestMarker.Name)]
public class PayCashflowComponent(ServerForTest server, ITestOutputHelper testOutputHelper)
{
    private const string ValidationErrorTitle = "One or more validation errors occurred.";
    private static readonly DateOnly Scheduled = new(2026, 11, 1);
    private static readonly DateOnly PaidOn = new(2026, 11, 3);

    private readonly ScenarioOutput _scenarioOutput = new(testOutputHelper);

    private User _user = null!;
    private Cashflow _cashflow = null!;

    [Fact]
    public Task WhenPayingAsScheduled() =>
        ScenarioRunner.Create("Paying an upcoming cashflow", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a cashflow", TheUserHasACashflow)
            .When("the user pays it", () => Paying(_cashflow.Id.Value, "1500.00", updateRecurringAmount: "false"))
            .Then<HttpResponseMessage>("the payment id is returned", ThePaymentIdIsReturned)
            .And("the payment is recorded for the occurrence", () => ThePaymentIsRecorded(1500m))
            .And("the recurring amount is unchanged", () => TheRecurringAmountIs(1450m))
            .PlayAsync();

    [Fact]
    public Task WhenUpdatingTheRecurringAmount() =>
        ScenarioRunner.Create("Paying an upcoming cashflow and updating its recurring amount", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a cashflow", TheUserHasACashflow)
            .When("the user pays it with a new recurring amount", () => Paying(_cashflow.Id.Value, "1500.00", updateRecurringAmount: "true"))
            .Then<HttpResponseMessage>("the payment id is returned", ThePaymentIdIsReturned)
            .And("the recurring amount is the amount paid", () => TheRecurringAmountIs(1500m))
            .PlayAsync();

    [Fact]
    public Task WhenTheAmountIsInvalid() =>
        ScenarioRunner.Create("Paying an upcoming cashflow with an invalid amount", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("the user has a cashflow", TheUserHasACashflow)
            .When("the user pays it with a negative amount", () => Paying(_cashflow.Id.Value, "-1", updateRecurringAmount: null))
            .Then<HttpResponseMessage>("a validation error is returned on the amount",
                response => AValidationErrorIsReturnedOn(response, MoneyErrors.CodeInvalid))
            .PlayAsync();

    [Fact]
    public Task WhenTheCashflowBelongsToAnotherUser() =>
        ScenarioRunner.Create("Paying another user's cashflow", _scenarioOutput)
            .Given("a registered user", ARegisteredUser)
            .And("another user has a cashflow", AnotherUserHasACashflow)
            .When("the user pays the other user's cashflow", () => Paying(_cashflow.Id.Value, "1450.00", updateRecurringAmount: null))
            .Then<HttpResponseMessage>("the cashflow is not found", TheCashflowIsNotFound)
            .And("no payment is recorded", NoPaymentIsRecorded)
            .PlayAsync();

    [Fact]
    public Task WhenTheUserIsNotRegistered() =>
        ScenarioRunner.Create("Paying a cashflow as a user who never registered", _scenarioOutput)
            .Given("an authenticated user who is not registered", AnUnregisteredUser)
            .And("another user has a cashflow", AnotherUserHasACashflow)
            .When("the user pays the other user's cashflow", () => Paying(_cashflow.Id.Value, "1450.00", updateRecurringAmount: null))
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

    private async Task<HttpResponseMessage> Paying(Guid cashflowId, string amount, string? updateRecurringAmount)
    {
        using var client = server.CreateClientFor(_user.IdentityObjectId);
        return await client.PostAsJsonAsync(
            new Uri("/api/v2/cashflows/pay-cashflow", UriKind.Relative),
            new
            {
                CashflowId = cashflowId.ToString(),
                Date = "2026-11-03",
                Amount = amount,
                CashflowDate = "2026-11-01",
                UpdateRecurringAmount = updateRecurringAmount
            });
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

    private async Task ThePaymentIsRecorded(decimal amount) =>
        (await server.Database.PaymentsInDbAsync(_cashflow.Id)).Should().Equal(
            _cashflow.Pay(Id.Valid(_paymentId), PaidOn, Money.Valid(amount), Scheduled));

    private async Task TheRecurringAmountIs(decimal amount) =>
        (await server.Database.CashflowInDbAsync(_cashflow.Id))!.Amount.Should().Be(Money.Valid(amount));

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
