using DrifterApps.Seeds.FluentResult;

namespace Holefeeder.Service.Shared.Core;

internal record Id
{
    public Guid Value { get; private init; }

    public static Result<Id> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            return IdErrors.Invalid;
        }

        return new Id(value);
    }

    public static Id Valid(Guid value) => new(value);

    public static Id Empty => new(Guid.Empty);

    private Id(Guid value) => Value = value;
}

internal static class IdErrors
{
    public const string CodeInvalid = "id-invalid";

    public static ResultError Invalid => new(CodeInvalid, "Id cannot be empty.");
}
