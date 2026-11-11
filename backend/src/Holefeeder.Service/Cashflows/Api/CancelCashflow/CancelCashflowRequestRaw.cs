using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Core.CancelCashflow;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Cashflows.Api.CancelCashflow;

internal sealed record CancelCashflowRequestRaw([FromBody] CancelCashflowBody Body)
{
    public static Result<CancelCashflowRequest> ToCancelCashflowRequest(CancelCashflowRequestRaw raw, User user)
    {
        var cashflowId = Id.Create(raw.Body.CashflowId);

        var validation = ResultAggregate.Create();
        if (cashflowId.IsFailure)
        {
            validation.AddResult(cashflowId.Error);
        }

        return validation.OnSuccess<CancelCashflowRequest>(() => new CancelCashflowRequest(user, cashflowId.Value));
    }
}
