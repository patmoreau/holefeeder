using System.Text.Json.Serialization;

using Holefeeder.Service.Shared.Api;

namespace Holefeeder.Service.Accounts.Api.FavoriteAccount;

internal sealed record FavoriteAccountBody(
    string? Id,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? IsFavorite);
