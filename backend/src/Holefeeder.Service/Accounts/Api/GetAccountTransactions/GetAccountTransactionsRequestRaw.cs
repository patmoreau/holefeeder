using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core.GetAccountTransactions;
using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Shared.Core.Search;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Accounts.Api.GetAccountTransactions;

internal sealed record GetAccountTransactionsRequestRaw(
    [FromRoute(Name = "id")] string IdRaw,
    [FromQuery(Name = "offset")] string? OffsetRaw,
    [FromQuery(Name = "limit")] string? LimitRaw)
{
    public static Result<GetAccountTransactionsRequest> ToGetAccountTransactionsRequest(GetAccountTransactionsRequestRaw raw, User user)
    {
        var id = Id.Create(raw.IdRaw);
        var offset = Offset.Create(raw.OffsetRaw);
        var limit = Limit.Create(raw.LimitRaw);

        var validation = ResultAggregate.Create();
        if (id.IsFailure)
        {
            validation.AddResult(id.Error);
        }

        if (offset.IsFailure)
        {
            validation.AddResult(offset.Error);
        }

        if (limit.IsFailure)
        {
            validation.AddResult(limit.Error);
        }

        return validation.OnSuccess<GetAccountTransactionsRequest>(() =>
            new GetAccountTransactionsRequest(user, id.Value, offset.Value, limit.Value));
    }
}
