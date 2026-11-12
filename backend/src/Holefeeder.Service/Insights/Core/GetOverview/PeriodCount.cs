using System.Globalization;

using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Insights.Core.GetOverview;

internal sealed record PeriodCount
{
    public static readonly IReadOnlyList<int> Offered = [3, 6, 12];

    private PeriodCount(int value) => Value = value;

    public int Value { get; }

    public static Result<PeriodCount> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return new PeriodCount(6);
        }

        return int.TryParse(value.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var count) && Offered.Contains(count)
            ? new PeriodCount(count)
            : PeriodCountErrors.Invalid;
    }

    public static PeriodCount Valid(int value) => new(value);
}

internal static class PeriodCountErrors
{
    public const string CodeInvalid = "insights-period-count-invalid";

    public static ResultError Invalid => new(CodeInvalid, "The number of periods must be 3, 6 or 12.");
}
