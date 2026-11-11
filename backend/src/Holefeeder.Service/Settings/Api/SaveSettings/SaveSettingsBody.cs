using System.Text.Json.Serialization;

using Holefeeder.Service.Shared.Api;

namespace Holefeeder.Service.Settings.Api.SaveSettings;

internal sealed record SaveSettingsBody(
    string? EffectiveDate,
    string? IntervalType,
    [property: JsonConverter(typeof(JsonScalarAsText))] string? Frequency);
