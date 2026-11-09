using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.StoreItems.Core.GetStoreItem;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.StoreItems.Api.GetStoreItem;

internal sealed record GetStoreItemRequestRaw([FromRoute(Name = "id")] string IdRaw)
{
    public static Result<GetStoreItemRequest> ToGetStoreItemRequest(GetStoreItemRequestRaw raw, User user)
    {
        var id = Id.Create(raw.IdRaw);

        var validation = ResultAggregate.Create();
        if (id.IsFailure)
        {
            validation.AddResult(id.Error);
        }

        return validation.OnSuccess<GetStoreItemRequest>(() => new GetStoreItemRequest(user, id.Value));
    }
}
