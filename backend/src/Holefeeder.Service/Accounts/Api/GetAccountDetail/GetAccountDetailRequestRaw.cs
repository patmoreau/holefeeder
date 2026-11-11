using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core.GetAccountDetail;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Accounts.Api.GetAccountDetail;

internal sealed record GetAccountDetailRequestRaw([FromRoute(Name = "id")] string IdRaw)
{
    public static Result<GetAccountDetailRequest> ToGetAccountDetailRequest(GetAccountDetailRequestRaw raw, User user, DateOnly today)
    {
        var id = Id.Create(raw.IdRaw);

        var validation = ResultAggregate.Create();
        if (id.IsFailure)
        {
            validation.AddResult(id.Error);
        }

        return validation.OnSuccess<GetAccountDetailRequest>(() => new GetAccountDetailRequest(user, id.Value, today));
    }
}
