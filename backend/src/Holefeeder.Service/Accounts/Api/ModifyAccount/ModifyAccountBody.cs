using System.Text.Json.Serialization;

namespace Holefeeder.Service.Accounts.Api.ModifyAccount;

internal sealed record ModifyAccountBody(
    string? Id,
    string? Name,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? OpenBalance,
    string? Description);
