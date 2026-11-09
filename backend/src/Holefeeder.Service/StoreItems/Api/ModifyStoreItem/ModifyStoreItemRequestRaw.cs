using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.ModifyStoreItem;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.StoreItems.Api.ModifyStoreItem;

internal sealed record ModifyStoreItemRequestRaw([FromBody] ModifyStoreItemBody Body)
{
    public static Result<ModifyStoreItemRequest> ToModifyStoreItemRequest(ModifyStoreItemRequestRaw raw, User user)
    {
        var id = Id.Create(raw.Body.Id);
        var data = Data.Create(raw.Body.Data);

        var validation = ResultAggregate.Create();
        if (id.IsFailure)
        {
            validation.AddResult(id.Error);
        }
        if (data.IsFailure)
        {
            validation.AddResult(data.Error);
        }

        return validation.OnSuccess<ModifyStoreItemRequest>(() => new ModifyStoreItemRequest(user, id.Value, data.Value));
    }
}
