namespace Holefeeder.Domain.ValueObjects;

public readonly struct Money : IEquatable<Money>
{
    public decimal Value { get; private init; }

    public decimal ToDecimal() => Value;

    public static Result<Money> Create(decimal value) => decimal.IsNegative(value)
        ? Result<Money>.Failure(MoneyValueErrors.NoNegativeValue)
        : Result<Money>.Success(new Money { Value = value });

    public static implicit operator decimal(Money value) => value.Value;

    public static Money Zero => new() { Value = decimal.Zero };

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Money left, Money right) => left.Equals(right);

    public static bool operator !=(Money left, Money right) => !(left == right);

    public bool Equals(Money other) => Value == other.Value;

    public override bool Equals(object? obj) =>
        obj switch
        {
            Money other => Value == other.Value,
            decimal otherDecimal => Value == otherDecimal,
            _ => false
        };
}

public static class MoneyValueErrors
{
    public const string CodeNoNegativeValue = $"{nameof(Money)}.NoNegativeValue";

    public static ResultError NoNegativeValue => new(CodeNoNegativeValue, "Money values cannot be negative");
}
