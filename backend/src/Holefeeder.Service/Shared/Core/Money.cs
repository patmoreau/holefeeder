using System.Globalization;

using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Shared.Core;

internal sealed record Money
{
    private Money(decimal value) => Value = value;

    public decimal Value { get; }

    public static Money Zero => new(0m);

    public static Result<Money> Create(string? value) =>
        decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var amount) && amount.Scale <= 2
            ? new Money(amount)
            : MoneyErrors.Invalid;

    public static Money Valid(decimal value) => new(value);
}

internal static class MoneyErrors
{
    public const string CodeInvalid = "amount-invalid";

    public static ResultError Invalid => new(CodeInvalid, "Amount must be zero or more, with at most two decimals.");
}
