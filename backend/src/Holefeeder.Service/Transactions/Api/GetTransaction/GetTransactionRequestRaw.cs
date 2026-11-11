using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core.GetTransaction;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Transactions.Api.GetTransaction;

internal sealed record GetTransactionRequestRaw([FromRoute(Name = "id")] string IdRaw)
{
    public static Result<GetTransactionRequest> ToGetTransactionRequest(GetTransactionRequestRaw raw, User user)
    {
        var id = Id.Create(raw.IdRaw);

        var validation = ResultAggregate.Create();
        if (id.IsFailure)
        {
            validation.AddResult(id.Error);
        }

        return validation.OnSuccess<GetTransactionRequest>(() => new GetTransactionRequest(user, id.Value));
    }
}
