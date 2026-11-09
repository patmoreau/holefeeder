using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.CreateStoreItem;

using Microsoft.AspNetCore.Mvc;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.StoreItems.Api.CreateStoreItem;

internal sealed record CreateStoreItemRequestRaw([FromBody] CreateStoreItemBody Body)
{
    public static Result<CreateStoreItemRequest> ToCreateStoreItemRequest(CreateStoreItemRequestRaw raw, User user)
    {
        var code = Code.Create(raw.Body.Code);
        var data = Data.Create(raw.Body.Data);

        var validation = ResultAggregate.Create();
        if (code.IsFailure)
        {
            validation.AddResult(code.Error);
        }
        if (data.IsFailure)
        {
            validation.AddResult(data.Error);
        }

        return validation.OnSuccess<CreateStoreItemRequest>(() => new CreateStoreItemRequest(user, code.Value, data.Value));
    }
}
