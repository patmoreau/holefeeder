using Holefeeder.Domain.Enumerations;

namespace Holefeeder.Domain.ValueObjects;

public record UserSettings
{
    public DateOnly EffectiveDate { get; init; }
    public DateIntervalType IntervalType { get; init; } = DateIntervalType.Monthly;
    public int Frequency { get; init; }

    public static UserSettings Default => new()
    {
        EffectiveDate = DateOnly.FromDateTime(DateTime.Now),
        IntervalType = DateIntervalType.Monthly,
        Frequency = 1
    };
}
