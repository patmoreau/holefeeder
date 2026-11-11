using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Dashboard.Core;

internal sealed record PeriodSettings
{
    private PeriodSettings(DateOnly effectiveDate, IntervalType intervalType, int frequency) =>
        (EffectiveDate, IntervalType, Frequency) = (effectiveDate, intervalType, frequency);

    public DateOnly EffectiveDate { get; }

    public IntervalType IntervalType { get; }

    public int Frequency { get; }

    public static Result<PeriodSettings> Create(DateOnly effectiveDate, IntervalType intervalType, int frequency) =>
        frequency < 1
            ? PeriodSettingsErrors.FrequencyInvalid
            : new PeriodSettings(effectiveDate, intervalType, frequency);

    public static PeriodSettings Valid(DateOnly effectiveDate, IntervalType intervalType, int frequency) =>
        new(effectiveDate, intervalType, frequency);

    public static PeriodSettings Default(DateOnly today) => new(today, IntervalType.Monthly, 1);
}

internal static class PeriodSettingsErrors
{
    public const string CodeFrequencyInvalid = "period-settings-frequency-invalid";

    public static ResultError FrequencyInvalid => new(CodeFrequencyInvalid, "Frequency must be at least 1.");
}
