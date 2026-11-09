using System.Globalization;

using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Shared.Core.Search;

internal sealed record Limit
{
    public const int Max = 100;

    public int Value { get; }

    public static readonly Limit Default = new(20);

    public static Result<Limit> Create(int value) =>
        value switch
        {
            < 1 or > Max => LimitErrors.Invalid,
            _ => new Limit(value)
        };

    public static Result<Limit> Create(string? value) =>
        value is null ? Default
        : int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number) ? Create(number)
        : LimitErrors.Invalid;

    public static Limit Valid(int value) => new(value);

    private Limit(int value) => Value = value;
}

internal static class LimitErrors
{
    public const string CodeInvalid = "limit-invalid";

    public static ResultError Invalid => new(CodeInvalid, $"Limit must be greater than zero and less than or equal to {Limit.Max}.");
}
