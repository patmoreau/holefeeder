using System.Globalization;

using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Transactions.Core;

internal sealed record Frequency
{
    private Frequency(int value) => Value = value;

    public int Value { get; }

    public static Result<Frequency> Create(string? value) =>
        int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var frequency) && frequency >= 1
            ? new Frequency(frequency)
            : FrequencyErrors.Invalid;

    public static Frequency Valid(int value) => new(value);
}

internal static class FrequencyErrors
{
    public const string CodeInvalid = "frequency-invalid";

    public static ResultError Invalid => new(CodeInvalid, "Frequency must be a whole number of 1 or more.");
}
