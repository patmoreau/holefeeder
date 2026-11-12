using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Cashflows.Core.ModifyCashflow;
using Holefeeder.Service.Shared.Api;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Cashflows.Api.ModifyCashflow;

internal sealed record ModifyCashflowRequestRaw([FromBody] ModifyCashflowBody Body)
{
    public static Result<ModifyCashflowRequest> ToModifyCashflowRequest(ModifyCashflowRequestRaw raw, User user)
    {
        var id = Id.Create(raw.Body.Id);
        var amount = Money.Create(raw.Body.Amount);
        var effectiveDate = IsoDate.Parse(raw.Body.EffectiveDate, ModifyCashflowRequestErrors.EffectiveDateInvalid);

        var validation = ResultAggregate.Create();
        if (id.IsFailure)
        {
            validation.AddResult(id.Error);
        }
        if (amount.IsFailure)
        {
            validation.AddResult(amount.Error);
        }
        if (effectiveDate.IsFailure)
        {
            validation.AddResult(effectiveDate.Error);
        }

        return validation.OnSuccess<ModifyCashflowRequest>(() => new ModifyCashflowRequest(
            user, id.Value, amount.Value, effectiveDate.Value, raw.Body.Description, TagList.Create(raw.Body.Tags)));
    }
}

internal static class ModifyCashflowRequestErrors
{
    public const string CodeEffectiveDateInvalid = "cashflow-effective-date-invalid";

    public static ResultError EffectiveDateInvalid => new(CodeEffectiveDateInvalid, "Effective date must be a date as YYYY-MM-DD.");
}
