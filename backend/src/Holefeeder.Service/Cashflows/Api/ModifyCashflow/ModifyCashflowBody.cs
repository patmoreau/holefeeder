using System.Text.Json.Serialization;

using Holefeeder.Service.Shared.Api;

namespace Holefeeder.Service.Cashflows.Api.ModifyCashflow;

internal sealed record ModifyCashflowBody(
    string? Id,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? Amount,
    string? EffectiveDate,
    string? Description,
    string?[]? Tags);
