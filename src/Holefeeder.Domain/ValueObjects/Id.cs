using System.Diagnostics.CodeAnalysis;

using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Domain.ValueObjects;

public readonly struct Id : IEquatable<Id>, IPrimitiveType<Guid>, IParsable<Id>
{
    public Guid Value { get; private init; }

    public Guid ToGuid() => Value;

    public static Result<Id> Create(Guid value) => Guid.Empty == value
        ? IdValueErrors.NoEmptyValue
        : new Id {Value = value};

    public static implicit operator Guid(Id value) => value.Value;

    public static implicit operator Id(Guid value) => Create(value).Value;

    public static Id Empty => new() {Value = Guid.Empty};

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Id left, Id right) => left.Equals(right);

    public static bool operator !=(Id left, Id right) => !(left == right);

    public bool Equals(Id other) => Value == other.Value;

    public override bool Equals(object? obj) =>
        obj switch
        {
            Id other => Value == other.Value,
            Guid otherGuid => Value == otherGuid,
            _ => false
        };

    public Id ToId() => Value;

    public static Id Parse(string s, IFormatProvider? provider)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            throw new ArgumentException("Input string cannot be null or empty.", nameof(s));
        }

        var guid = Guid.Parse(s);
        var result = Create(guid);

        return result.IsSuccess ? result.Value : throw new ArgumentException(result.Error.Description, nameof(s));
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Id result)
    {
        result = Empty;

        if (string.IsNullOrWhiteSpace(s))
        {
            return false;
        }

        if (!Guid.TryParse(s, out var guid))
        {
            return false;
        }

        var createResult = Create(guid);
        if (createResult.IsSuccess)
        {
            result = createResult.Value;
            return true;
        }

        return false;
    }
}

public static class IdValueErrors
{
    public const string CodeNoEmptyValue = $"{nameof(Id)}.NoEmptyValue";

    public static ResultError NoEmptyValue => new(CodeNoEmptyValue, "Id values cannot be empty");
}
