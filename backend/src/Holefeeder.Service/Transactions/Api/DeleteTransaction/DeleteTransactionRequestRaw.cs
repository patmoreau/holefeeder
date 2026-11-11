using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.Transactions.Core.DeleteTransaction;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.Transactions.Api.DeleteTransaction;

internal sealed record DeleteTransactionRequestRaw([FromRoute(Name = "id")] string IdRaw)
{
    public static Result<DeleteTransactionRequest> ToDeleteTransactionRequest(DeleteTransactionRequestRaw raw, User user)
    {
        var id = Id.Create(raw.IdRaw);

        var validation = ResultAggregate.Create();
        if (id.IsFailure)
        {
            validation.AddResult(id.Error);
        }

        return validation.OnSuccess<DeleteTransactionRequest>(() => new DeleteTransactionRequest(user, id.Value));
    }
}
