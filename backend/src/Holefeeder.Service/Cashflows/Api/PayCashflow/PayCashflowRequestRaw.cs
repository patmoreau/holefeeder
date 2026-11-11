using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Core;
using Holefeeder.Service.Cashflows.Core.PayCashflow;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Cashflows.Api.PayCashflow;

internal sealed record PayCashflowRequestRaw([FromBody] PayCashflowBody Body)
{
    public static Result<PayCashflowRequest> ToPayCashflowRequest(PayCashflowRequestRaw raw, User user)
    {
        var cashflowId = Id.Create(raw.Body.CashflowId);
        var date = IsoDate.Parse(raw.Body.Date, IsoDateErrors.DateInvalid);
        var amount = Money.Create(raw.Body.Amount);
        var cashflowDate = IsoDate.Parse(raw.Body.CashflowDate, IsoDateErrors.CashflowDateInvalid);
        var updateRecurringAmount = ParseUpdateRecurringAmount(raw.Body.UpdateRecurringAmount);

        var validation = ResultAggregate.Create();
        if (cashflowId.IsFailure)
        {
            validation.AddResult(cashflowId.Error);
        }
        if (date.IsFailure)
        {
            validation.AddResult(date.Error);
        }
        if (amount.IsFailure)
        {
            validation.AddResult(amount.Error);
        }
        if (cashflowDate.IsFailure)
        {
            validation.AddResult(cashflowDate.Error);
        }
        if (updateRecurringAmount.IsFailure)
        {
            validation.AddResult(updateRecurringAmount.Error);
        }

        return validation.OnSuccess<PayCashflowRequest>(() => new PayCashflowRequest(
            user, cashflowId.Value, date.Value, amount.Value, cashflowDate.Value, updateRecurringAmount.Value));
    }

    private static Result<bool> ParseUpdateRecurringAmount(string? value) =>
        value switch
        {
            null => false,
            _ when bool.TryParse(value, out var update) => update,
            _ => PayCashflowRequestErrors.UpdateRecurringAmountInvalid
        };
}

internal static class PayCashflowRequestErrors
{
    public const string CodeUpdateRecurringAmountInvalid = "update-recurring-amount-invalid";

    public static ResultError UpdateRecurringAmountInvalid =>
        new(CodeUpdateRecurringAmountInvalid, "Update recurring amount must be true or false.");
}
