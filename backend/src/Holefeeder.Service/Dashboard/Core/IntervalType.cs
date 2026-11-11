using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Dashboard.Core;

internal sealed record IntervalType
{
    public static readonly IntervalType Daily = new(nameof(Daily));
    public static readonly IntervalType Weekly = new(nameof(Weekly));
    public static readonly IntervalType Monthly = new(nameof(Monthly));
    public static readonly IntervalType Yearly = new(nameof(Yearly));
    public static readonly IntervalType OneTime = new(nameof(OneTime));

    private static readonly IntervalType[] All = [Daily, Weekly, Monthly, Yearly, OneTime];

    private IntervalType(string name) => Name = name;

    public string Name { get; }

    public static Result<IntervalType> Create(string? value) =>
        All.FirstOrDefault(type => string.Equals(type.Name, value, StringComparison.OrdinalIgnoreCase)) is { } intervalType
            ? intervalType
            : IntervalTypeErrors.Invalid;

    public DateOnly AddIteration(DateOnly date, int iteration) =>
        Name switch
        {
            nameof(Daily) => date.AddDays(iteration),
            nameof(Weekly) => date.AddDays(7 * iteration),
            nameof(Monthly) => date.AddMonths(iteration),
            nameof(Yearly) => date.AddYears(iteration),
            _ => date
        };
}

internal static class IntervalTypeErrors
{
    public const string CodeInvalid = "interval-type-invalid";

    public static ResultError Invalid => new(CodeInvalid, "Interval type is invalid.");
}
