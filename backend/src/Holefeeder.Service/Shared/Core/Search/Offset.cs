using System.Globalization;

using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Shared.Core.Search;

internal sealed record Offset
{
    public int Value { get; }

    public static readonly Offset Default = new(0);

    public static Result<Offset> Create(int value) =>
        value switch
        {
            < 0 => OffsetErrors.Invalid,
            _ => new Offset(value)
        };

    public static Result<Offset> Create(string? value) =>
        value is null ? Default
        : int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number) ? Create(number)
        : OffsetErrors.Invalid;

    public static Offset Valid(int value) => new(value);

    private Offset(int value) => Value = value;
}

internal static class OffsetErrors
{
    public const string CodeInvalid = "offset-invalid";

    public static ResultError Invalid => new(CodeInvalid, "Offset must be greater than or equal to zero.");
}
