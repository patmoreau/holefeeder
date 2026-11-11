using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Core.ClearCashflow;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Cashflows.Api.ClearCashflow;

internal sealed record ClearCashflowRequestRaw([FromBody] ClearCashflowBody Body)
{
    public static Result<ClearCashflowRequest> ToClearCashflowRequest(ClearCashflowRequestRaw raw, User user)
    {
        var cashflowId = Id.Create(raw.Body.CashflowId);
        var cashflowDate = IsoDate.Parse(raw.Body.CashflowDate, IsoDateErrors.CashflowDateInvalid);

        var validation = ResultAggregate.Create();
        if (cashflowId.IsFailure)
        {
            validation.AddResult(cashflowId.Error);
        }
        if (cashflowDate.IsFailure)
        {
            validation.AddResult(cashflowDate.Error);
        }

        return validation.OnSuccess<ClearCashflowRequest>(() => new ClearCashflowRequest(user, cashflowId.Value, cashflowDate.Value));
    }
}
