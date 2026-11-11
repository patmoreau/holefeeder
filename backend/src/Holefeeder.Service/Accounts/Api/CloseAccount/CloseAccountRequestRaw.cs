using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Accounts.Core.CloseAccount;
using Holefeeder.Service.Shared.Core;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Accounts.Api.CloseAccount;

internal sealed record CloseAccountRequestRaw([FromBody] CloseAccountBody Body)
{
    public static Result<CloseAccountRequest> ToCloseAccountRequest(CloseAccountRequestRaw raw, User user)
    {
        var id = Id.Create(raw.Body.Id);

        var validation = ResultAggregate.Create();
        if (id.IsFailure)
        {
            validation.AddResult(id.Error);
        }

        return validation.OnSuccess<CloseAccountRequest>(() => new CloseAccountRequest(user, id.Value));
    }
}
