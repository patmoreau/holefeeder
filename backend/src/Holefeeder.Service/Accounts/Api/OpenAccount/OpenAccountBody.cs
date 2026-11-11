using System.Text.Json.Serialization;

using Holefeeder.Service.Shared.Api;

namespace Holefeeder.Service.Accounts.Api.OpenAccount;

internal sealed record OpenAccountBody(
    string? Type,
    string? Name,
    string? OpenDate,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? OpenBalance,
    string? Description);
