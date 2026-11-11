using System.Text.Json.Serialization;

namespace Holefeeder.Service.Accounts.Api.ModifyAccount;

internal sealed record ModifyAccountBody(
    string? Id,
    string? Name,
    string? Type,
    string? OpenDate,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? OpenBalance,
    string? Description);
