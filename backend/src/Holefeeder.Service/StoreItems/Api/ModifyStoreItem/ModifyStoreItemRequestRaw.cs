using System.Text.Json.Serialization;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.Shared.Core;
using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.ModifyStoreItem;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.StoreItems.Api.ModifyStoreItem;

internal sealed record ModifyStoreItemRequestRaw(
    [property: JsonPropertyName("id")] string? IdRaw,
    [property: JsonPropertyName("data")] string? DataRaw)
{
    public static Result<ModifyStoreItemRequest> ToModifyStoreItemRequest(ModifyStoreItemRequestRaw raw, User user)
    {
        var id = Id.Create(raw.IdRaw);
        var data = Data.Create(raw.DataRaw);

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
