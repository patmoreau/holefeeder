namespace Holefeeder.Service.Settings.Api.GetSettings;

internal sealed record SettingsResponse(DateOnly EffectiveDate, string IntervalType, int Frequency);
