using System.Text.Json.Serialization;

using DrifterApps.Seeds.FluentResult;

using Holefeeder.Service.StoreItems.Core;
using Holefeeder.Service.StoreItems.Core.CreateStoreItem;

using User = Holefeeder.Service.Support.Core.User;

namespace Holefeeder.Service.StoreItems.Api.CreateStoreItem;

internal sealed record CreateStoreItemRequestRaw(
    [property: JsonPropertyName("code")] string? CodeRaw,
    [property: JsonPropertyName("data")] string? DataRaw)
{
    public static Result<CreateStoreItemRequest> ToCreateStoreItemRequest(CreateStoreItemRequestRaw raw, User user)
    {
        var code = Code.Create(raw.CodeRaw);
        var data = Data.Create(raw.DataRaw);

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
